using System;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;
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

            ApplyMigrationsTolerantly(dbContext, logger);

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

    private static void ApplyMigrationsTolerantly(ApplicationDbContext dbContext, ILogger logger)
    {
        var migrations = dbContext.Database.GetMigrations().ToList();

        foreach (var migration in migrations)
        {
            var applied = dbContext.Database.GetAppliedMigrations().ToHashSet();
            if (applied.Contains(migration))
                continue;

            try
            {
                dbContext.Database.Migrate(migration);
                logger.LogInformation("مایگریشن {Migration} اعمال شد.", migration);
            }
            catch (SqlException ex) when (IsAlreadyExistsError(ex))
            {
                RecordMigrationAsApplied(dbContext, migration);
                logger.LogWarning(ex,
                    "مایگریشن {Migration} از قبل در بک آپ قدیمی اعمال شده بود؛ به عنوان اعمال شده ثبت شد.",
                    migration);
            }
        }
    }

    private static bool IsAlreadyExistsError(SqlException ex)
    {
        return ex.Errors.Cast<SqlError>().Any(e =>
            e.Number == 2714 ||   // There is already an object named 'X'
            e.Number == 2601 ||   // Cannot insert duplicate key row
            e.Number == 2627);    // Violation of UNIQUE KEY constraint
    }

    private static void RecordMigrationAsApplied(ApplicationDbContext dbContext, string migrationId)
    {
        var sql = new StringBuilder();
        sql.Append("IF OBJECT_ID(N'dbo.__EFMigrationsHistory', N'U') IS NULL ");
        sql.Append("CREATE TABLE dbo.__EFMigrationsHistory (MigrationId nvarchar(150) NOT NULL CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY, ProductVersion nvarchar(32) NOT NULL);");
        sql.Append($"IF NOT EXISTS (SELECT 1 FROM dbo.__EFMigrationsHistory WHERE MigrationId = N'{migrationId}') ");
        sql.Append($"INSERT INTO dbo.__EFMigrationsHistory (MigrationId, ProductVersion) VALUES (N'{migrationId}', N'9.0.0');");

        dbContext.Database.ExecuteSqlRaw(sql.ToString());
    }
}
