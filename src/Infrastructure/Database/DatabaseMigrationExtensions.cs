using System;
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
}
