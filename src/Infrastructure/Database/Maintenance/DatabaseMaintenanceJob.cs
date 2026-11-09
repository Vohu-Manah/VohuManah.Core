using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Infrastructure.Database.Maintenance;





internal sealed class DatabaseMaintenanceJob(
    IServiceScopeFactory scopeFactory,
    ILogger<DatabaseMaintenanceJob> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(24);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken).ConfigureAwait(false);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using IServiceScope scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider
                    .GetRequiredService<Database.ApplicationDbContext>();

                await dbContext.Database
                    .ExecuteSqlRawAsync("EXEC dbo.usp_MaintainDatabase;", stoppingToken)
                    .ConfigureAwait(false);

                logger.LogInformation("نگهداری و کوچک\u200Cسازی دیتابیس انجام شد.");
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "اجرای جاب نگهداری دیتابیس با خطا مواجه شد.");
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
