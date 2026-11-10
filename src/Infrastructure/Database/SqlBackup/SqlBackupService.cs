using System.Threading;
using System;
using Application.Abstractions.Backup;
using Application.Library.Backup.Shared;
using Infrastructure.Database;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace VohuManah.Infrastructure.Database.SqlBackup;

internal sealed class SqlBackupService(
    ApplicationDbContext dbContext,
    ILogger<SqlBackupService> logger) : IBackupService
{
    private const string SpName = "dbo.usp_BackupDatabase";





    private static string BackupFolder => Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "sqlBackups");

    
    
    
    private static readonly string? SqlBackupFolder = Environment.GetEnvironmentVariable("SQL_BACKUP_FOLDER");

    private static string ToSqlPath(string webApiPath)
    {
        if (string.IsNullOrEmpty(SqlBackupFolder) ||
            !webApiPath.StartsWith(BackupFolder + Path.DirectorySeparatorChar, StringComparison.Ordinal))
        {
            return webApiPath;
        }
        return Path.Combine(SqlBackupFolder, webApiPath[(BackupFolder.Length + 1)..]);
    }

    private static readonly Lock ProgressLock = new();
    private static BackupProgress _progress = new(false, 0, string.Empty, null, null, Array.Empty<string>());


    private static readonly List<string> ProgressLog = new();






    private static CancellationTokenSource? _runningCts;







    private static SqlCommand? _runningCommand;

    public BackupProgress GetProgress()
    {
        lock (ProgressLock)
        {
            return _progress with { Log = ProgressLog.ToArray() };
        }
    }


    public async Task<bool> CancelAsync(CancellationToken cancellationToken)
    {
        CancellationTokenSource? cts = _runningCts;
        if (cts is null || cts.IsCancellationRequested)
        {
            return false;
        }

        await cts.CancelAsync().ConfigureAwait(false);





        _runningCommand?.Cancel();


        await KillBackupSessionAsync().ConfigureAwait(false);

        AppendLog("درخواست لغو بکاپ ثبت شد.");
        return true;
    }

    private static void AppendLog(string line)
    {
        lock (ProgressLock)
        {
            ProgressLog.Add($"{DateTime.Now:HH:mm:ss} — {line}");

            if (ProgressLog.Count > 200)
            {
                ProgressLog.RemoveRange(0, ProgressLog.Count - 200);
            }
        }
    }

    public Task<BackupFileInfo> GetBackupInfoAsync(string fileName, CancellationToken cancellationToken)
    {
        var file = new FileInfo(BuildPath(fileName));
        if (!file.Exists)
        {
            return Task.FromResult(new BackupFileInfo(false, fileName, string.Empty, string.Empty, 0));
        }

        DateTime lastWrite = file.LastWriteTime;
        var info = new BackupFileInfo(
            true,
            fileName,
            lastWrite.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture),
            lastWrite.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
            file.Length);

        return Task.FromResult(info);
    }

    public async Task<BackupFileInfo> GetExistingBackupAsync(CancellationToken cancellationToken)
    {


        BackupFileInfo oneTime = await GetBackupInfoAsync(BackupFileNames.OneTime, cancellationToken)
            .ConfigureAwait(false);

        if (oneTime.Exists)
        {
            return oneTime;
        }

        return await GetBackupInfoAsync(BackupFileNames.Daily, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Stream> OpenReadAsync(string fileName, bool deleteAfterRead, CancellationToken cancellationToken)
    {
        string path = BuildPath(fileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("فایل بکاپ یافت نشد", path);
        }



        if (deleteAfterRead)
        {
            logger.LogInformation("فایل بکاپ {FileName} پس از بستن stream از سرور حذف خواهد شد.", fileName);
            return new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 64 * 1024,
                options: FileOptions.Asynchronous | FileOptions.DeleteOnClose);
        }

        return new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 64 * 1024,
            options: FileOptions.Asynchronous);
    }

    public async Task<byte[]> ReadFileAsync(string fileName, bool deleteAfterRead, CancellationToken cancellationToken)
    {
        byte[] content = await File.ReadAllBytesAsync(BuildPath(fileName), cancellationToken).ConfigureAwait(false);

        if (deleteAfterRead)
        {
            try
            {
                File.Delete(BuildPath(fileName));
                logger.LogInformation("فایل بکاپ {FileName} پس از دانلود از سرور حذف شد.", fileName);
            }
            catch (IOException ex)
            {
                logger.LogWarning(ex, "حذف فایل بکاپ {FileName} ناموفق بود؛ در اجرای بعدی جاب پاک میشود.", fileName);
            }
        }

        return content;
    }

    public async Task RunBackupAsync(string fileName, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(BackupFolder);

        string path = BuildPath(fileName);



        using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        CancellationTokenSource? previous = Interlocked.Exchange(ref _runningCts, cts);

        lock (ProgressLock)
        {
            ProgressLog.Clear();
        }

        AppendLog($"شروع بکاپ: {fileName}");
        SetProgress(true, 5, "آماده سازی بکاپ", fileName, null);

        try
        {
            AppendLog("اتصال به دیتابیس...");
            SqlConnection connection = (SqlConnection)dbContext.Database.GetDbConnection()
                ?? throw new InvalidOperationException("اتصال دیتابیس برقرار نشد");
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cts.Token).ConfigureAwait(false);
            }


            AppendLog("اجرای stored procedure بکاپ...");
            await using SqlCommand command = CreateBackupCommand(connection, path);
            Interlocked.Exchange(ref _runningCommand, command);


            SqlInfoMessageEventHandler onInfo = (_, e) =>
            {
                Match match = Regex.Match(e.Message ?? string.Empty, @"(\d+)\s+percent processed", RegexOptions.IgnoreCase);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int percent))
                {
                    SetProgress(true, percent, "در حال بکاپگیری از دیتابیس", fileName, null);
                    AppendLog($"پیشرفت بکاپ: {percent}٪");
                }
            };

            connection.InfoMessage += onInfo;
            try
            {
                await command.ExecuteNonQueryAsync(cts.Token).ConfigureAwait(false);
            }
            finally
            {
                connection.InfoMessage -= onInfo;
            }

            SetProgress(false, 100, "بکاپ با موفقیت انجام شد", fileName, null);
            AppendLog("بکاپ با موفقیت انجام شد.");
            logger.LogInformation("بکاپ دیتابیس در {Path} ذخیره شد.", path);
        }
        catch (OperationCanceledException ex)
        {

            AppendLog("بکاپ لغو شد.");
            SetProgress(false, 0, "بکاپ لغو شد", fileName, "عملیات بکاپ توسط کاربر لغو شد.");
            TryDeletePartialBackup(path);
            logger.LogWarning(ex, "بکاپ دیتابیس توسط کاربر لغو شد.");
        }
        catch (Exception ex) when (ex is OperationCanceledException
            || ex.InnerException is OperationCanceledException
            || ex.Message.Contains("backup or restore was aborted", StringComparison.OrdinalIgnoreCase))
        {


            AppendLog("بکاپ لغو شد.");
            SetProgress(false, 0, "بکاپ لغو شد", fileName, "عملیات بکاپ توسط کاربر لغو شد.");
            TryDeletePartialBackup(path);
            logger.LogWarning(ex, "بکاپ دیتابیس توسط کاربر لغو شد.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "بکاپگیری دیتابیس ناموفق بود.");
            AppendLog("خطا: " + ex.Message);
            SetProgress(false, 0, "خطا در بکاپگیری", fileName, "بکاپگیری با خطا مواجه شد: " + ex.Message);
            throw new InvalidOperationException("اجرای بکاپ دیتابیس شکست خورد", ex);
        }
        finally
        {
            Interlocked.Exchange(ref _runningCommand, null);
            Interlocked.CompareExchange(ref _runningCts, null, cts);
            previous?.Dispose();
        }
    }






    private async Task KillBackupSessionAsync()
    {


        const string KillSql = @"
DECLARE @spid INT;
SELECT @spid = MIN(r.session_id)
FROM master.sys.dm_exec_requests r
WHERE r.session_id <> @@SPID
  AND r.command LIKE N'%BACKUP DATABASE%';
IF @spid IS NOT NULL
BEGIN
    DECLARE @kill NVARCHAR(50);
    SET @kill = N'KILL ' + CAST(@spid AS VARCHAR(10));
    EXEC(@kill);
END";

        SqlConnectionStringBuilder builder = new(
            ((SqlConnection)dbContext.Database.GetDbConnection()).ConnectionString);



        builder.InitialCatalog = "master";


        using CancellationTokenSource killCts = new(TimeSpan.FromSeconds(20));
        await using SqlConnection killConnection = new(builder.ConnectionString);
        await killConnection.OpenAsync(killCts.Token).ConfigureAwait(false);

        await using SqlCommand killCommand = new(KillSql, killConnection);
        killCommand.CommandTimeout = 15;
        await killCommand.ExecuteNonQueryAsync(killCts.Token).ConfigureAwait(false);

        await using SqlCommand probe = new(
            "SELECT COUNT(*) FROM master.sys.dm_exec_requests WHERE command LIKE N'%BACKUP DATABASE%'",
            killConnection);
        killCommand.CommandTimeout = 5;
        object? remaining = await probe.ExecuteScalarAsync(killCts.Token).ConfigureAwait(false);
        AppendLog($"KILL اجرا شد - sessionهاي باقی مانده: {remaining}");


        if (remaining is 0)
        {
            await using SqlCommand probe2 = new(
                "SELECT TOP 3 session_id, command, status FROM master.sys.dm_exec_requests WHERE command LIKE N'%BACKUP%'",
                killConnection);
            using SqlDataReader reader = await probe2.ExecuteReaderAsync(killCts.Token).ConfigureAwait(false);
            while (await reader.ReadAsync(killCts.Token).ConfigureAwait(false))
            {
                AppendLog($"باقي مانده: spid={reader.GetValue(0)} cmd={reader.GetValue(1)} status={reader.GetValue(2)}");
            }
        }
    }
    
    private static void TryDeletePartialBackup(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public async Task RestoreFromUploadAsync(Stream backupContent, string fileName, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(BackupFolder);


        string safeName = Path.GetFileName(fileName);
        string path = BuildPath(safeName);
        SetProgress(true, 10, "ذخیره فایل آپلودشده", safeName, null);

        await using (var target = File.Create(path))
        {
            await backupContent.CopyToAsync(target, cancellationToken).ConfigureAwait(false);
        }

        await RestoreAsync(path, safeName, cancellationToken).ConfigureAwait(false);
        File.Delete(path);
    }

    public async Task CleanupExpiredOneTimeBackupAsync(TimeSpan maxAge, CancellationToken cancellationToken)
    {
        string path = BuildPath(BackupFileNames.OneTime);
        FileInfo file = new(path);

        if (!file.Exists)
        {
            return;
        }

        if (DateTime.UtcNow - file.LastWriteTimeUtc < maxAge)
        {
            return;
        }

        try
        {
            file.Delete();
            logger.LogInformation(
                "فایل بکاپ لحظهای {FileName} چون بیش از {Hours} ساعت دانلود نشد حذف شد.",
                BackupFileNames.OneTime,
                maxAge.TotalHours);
        }
        catch (IOException ex)
        {
            logger.LogWarning(ex, "حذف فایل بکاپ لحظهای ناموفق بود.");
        }
    }

    public async Task RestoreDailyAsync(CancellationToken cancellationToken)
    {
        string path = BuildPath(BackupFileNames.Daily);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("فایل بکاپ روزانه یافت نشد.", path);
        }

        await RestoreAsync(path, BackupFileNames.Daily, cancellationToken).ConfigureAwait(false);
    }

    private async Task RestoreAsync(string path, string fileName, CancellationToken cancellationToken)
    {
        SetProgress(true, 30, "آماده سازی بازگردانی", fileName, null);



        string targetDatabase = dbContext.Database.GetDbConnection().Database;
        string masterConnectionString = BuildMasterConnectionString();

        await using var connection = new SqlConnection(masterConnectionString);

        try
        {
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            await using SqlCommand command = CreateRestoreCommand(connection, targetDatabase, path);

            SqlInfoMessageEventHandler onInfo = (_, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Message))
                {
                    logger.LogInformation("RESTORE: {Message}", e.Message);
                }
            };

            connection.InfoMessage += onInfo;
            try
            {
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                connection.InfoMessage -= onInfo;
            }


            SqlConnection.ClearAllPools();

            SetProgress(false, 100, "بازگردانی با موفقیت انجام شد؛ لطفا دوباره وارد شوید", fileName, null);
            logger.LogInformation("بازگردانی دیتابیس از {Path} انجام شد.", path);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "بازگردانی دیتابیس ناموفق بود.");
            SetProgress(false, 0, "خطا در بازگردانی", fileName, "بازگردانی با خطا مواجه شد: " + ex.Message);
            throw new InvalidOperationException("بازگردانی دیتابیس از فایل بکاپ شکست خورد", ex);
        }
    }





    private string BuildMasterConnectionString()
    {
        var builder = new SqlConnectionStringBuilder(dbContext.Database.GetConnectionString())
        {
            InitialCatalog = "master"
        };

        return builder.ConnectionString;
    }

    private static SqlCommand CreateBackupCommand(SqlConnection connection, string path)
    {
        var command = new SqlCommand(SpName, connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 0
        };

        command.Parameters.AddWithValue("@BackupPath", ToSqlPath(path));

        return command;
    }






    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Security",
        "CA2100:Review SQL queries for security vulnerabilities",
        Justification = "نام دیتابیس از connection string خوانده میشود، نه از ورودی کاربر؛ مسیر فایل پارامتری است")]
    private static SqlCommand CreateRestoreCommand(SqlConnection connection, string database, string path)
    {
        var command = new SqlCommand(
            "ALTER DATABASE [" + database + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
            "RESTORE DATABASE [" + database + "] FROM DISK = @path WITH REPLACE; " +
            "ALTER DATABASE [" + database + "] SET MULTI_USER;",
            connection)
        {
            CommandType = CommandType.Text,
            CommandTimeout = 0
        };

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@path";
        parameter.Value = ToSqlPath(path);
        command.Parameters.Add(parameter);

        return command;
    }

    private static string BuildPath(string fileName) => Path.Combine(BackupFolder, Path.GetFileName(fileName));

    private static void SetProgress(bool isRunning, int percent, string stage, string? fileName, string? error)
    {
        lock (ProgressLock)
        {
            _progress = new BackupProgress(isRunning, Math.Clamp(percent, 0, 100), stage, fileName, error, ProgressLog.ToArray());
        }
    }
}
