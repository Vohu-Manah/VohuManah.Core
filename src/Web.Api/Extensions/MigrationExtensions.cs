using Domain.Library;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Web.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            logger.LogInformation("Applying database migrations...");
            dbContext.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully.");

            // Check if admin user exists
            try
            {
                var adminExists = dbContext.Set<Domain.Library.User>()
                    .Any(u => u.UserName == "admin");

                if (adminExists)
                {
                    logger.LogInformation("Admin user exists in database.");
                }
                else
                {
                    logger.LogWarning("Admin user does not exist in database. Please ensure the EnsureAdminUserExists migration has been applied.");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Could not verify admin user existence. This is not critical.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply database migrations. The application will continue, but database operations may fail.");
            logger.LogError("Please ensure SQL Server is accessible and run migrations manually using: dotnet ef database update --project src/Infrastructure --startup-project src/Web.Api");
            // Don't throw - allow the application to start even if migrations fail
            // This is useful when the database is temporarily unavailable
        }
    }
}
