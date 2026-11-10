using System.Threading;
using Application.Abstractions.Backup;
using Application.Library.Backup.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace VohuManah.Infrastructure.Database.SqlBackup;






internal sealed class DailyBackupJob(
    IServiceScopeFactory scopeFactory,
    ILogger<DailyBackupJob> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(24);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        
        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken).ConfigureAwait(false);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using IServiceScope scope = scopeFactory.CreateScope();
                IBackupService backupService = scope.ServiceProvider.GetRequiredService<IBackupService>();

                await backupService.RunBackupAsync(BackupFileNames.Daily, stoppingToken).ConfigureAwait(false);

                logger.LogInformation("بکاپ روزانهٔ دیتابیس انجام شد.");
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "اجرای جاب بکاپ روزانه با خطا مواجه شد.");
            }

            try
            {
                await Task.Delay(Interval, stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}