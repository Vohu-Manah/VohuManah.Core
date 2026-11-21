using System;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VohuManah.Infrastructure.Database;

namespace VohuManah.Infrastructure.Database;

public static class DatabaseMigrationExtensions
{
    public static void ApplyMigrations(this IServiceProvider services, bool isDevelopment)
    {
        using IServiceScope scope = services.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            logger.LogInformation("در حال اعمال مایگریشن های دیتابیس...");

            dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));

            ReconcileBackupHistory(dbContext, logger);

            dbContext.Database.Migrate();

            logger.LogInformation("مایگریشن های دیتابیس با موفقیت اعمال شدند.");

            try
            {
                var adminExists = dbContext.LibraryUsers
                    .Any(u => u.UserName == "admin");

                if (adminExists)
                {
                    logger.LogInformation("کاربر ادمین در دیتابیس وجود دارد.");
                }
                else
                {
                    logger.LogWarning("کاربر ادمین در دیتابیس وجود ندارد. لطفاً اطمینان حاصل کنید که مایگریشن EnsureAdminUserExists اعمال شده است.");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "امکان بررسی وجود کاربر ادمین وجود نداشت. این مسئله حیاتی نیست.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "اعمال مایگریشن های دیتابیس با شکست مواجه شد.");

            if (isDevelopment)
            {
                logger.LogError("خطا در اعمال مایگریشن در محیط Development. برنامه متوقف می شود.");
                throw;
            }

            logger.LogError("برنامه ادامه خواهد یافت، اما عملیات دیتابیس ممکن است با خطا مواجه شوند.");
            logger.LogError("لطفاً اطمینان حاصل کنید که SQL Server در دسترس است و مایگریشن ها را به صورت دستی با دستور زیر اجرا کنید: " +
                            "dotnet ef database update --project src/Infrastructure --startup-project src/Web.Api");
        }
    }

    private static void ReconcileBackupHistory(ApplicationDbContext dbContext, ILogger logger)
    {
        var appInfoExists = dbContext.Database.SqlQueryRaw<int>(
                "SELECT CASE WHEN OBJECT_ID(N'dbo.AppInfo', N'U') IS NOT NULL THEN 1 ELSE 0 END AS [Value]")
            .Single() == 1;

        if (!appInfoExists)
            return;

        var migrationIds = dbContext.Database.GetMigrations().ToList();
        if (migrationIds.Count == 0)
            return;

        var sql = new StringBuilder();
        sql.Append("IF OBJECT_ID(N'dbo.__EFMigrationsHistory', N'U') IS NULL ");
        sql.Append("CREATE TABLE dbo.__EFMigrationsHistory (MigrationId nvarchar(150) NOT NULL CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY, ProductVersion nvarchar(32) NOT NULL);");
        foreach (var id in migrationIds)
        {
            sql.Append($"IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = N'{id}') ");
            sql.Append($"INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion) VALUES (N'{id}', N'9.0.0');");
        }

        dbContext.Database.ExecuteSqlRaw(sql.ToString());
        logger.LogWarning("بک آپ قدیمی بدون تاریخچه مایگریشن شناسایی شد. تاریخچه با وضعیت دیتابیس هماهنگ شد.");
    }
}
