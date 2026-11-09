using System.IO;
using System;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Infrastructure.Authentication;
using Infrastructure.Authorization;
using Infrastructure.Caching;
using Infrastructure.Database;
using Infrastructure.FileStorage;
using Infrastructure.Reports;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedKernel;
using System.Security.Claims;
using System.Text;
using VohuManah.Infrastructure.Database.SqlBackup;
using VohuManah.Infrastructure.Helper.Time;
using VohuManah.Infrastructure.Helper.HumanVerification;
using VohuManah.Infrastructure.Helper.PersianCalendar;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddServices()
            .AddDatabase(configuration)
            .AddHealthChecks(configuration)
            .AddAuthenticationInternal(configuration)
            .AddAuthorizationInternal();

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        services.AddSingleton<Application.Abstractions.Caching.ICacheManager, MemoryCacheManager>();

        services.AddSingleton<IPersianCalendar, PersianCalendarService>();
        services.AddScoped<PersianDateTime>();
        services.AddScoped<Application.Abstractions.FileStorage.IFileStorageService, LocalFileStorageService>();

        services.AddScoped<Application.Abstractions.Backup.IBackupService, SqlBackupService>();
        services.AddSingleton<Application.Abstractions.HumanVerification.IHumanVerificationService, HumanVerificationService>();
        services.AddHostedService<DailyBackupJob>();
        services.AddHostedService<BackupCleanupJob>();
        services.AddHostedService<Database.Maintenance.DatabaseMaintenanceJob>();

        return AddReportServices(services);
    }

    private static IServiceCollection AddReportServices(this IServiceCollection services)
    {
        services.AddSingleton<Infrastructure.Reports.Html.IReportResourceLoader, Infrastructure.Reports.Html.ReportResourceLoader>();
        services.AddTransient<Infrastructure.Reports.Html.IHtmlBuilder, Infrastructure.Reports.Html.HtmlBuilder>();
        services.AddScoped<Infrastructure.Reports.Html.IReportHtmlWrapper>(sp =>
        {
            var resourceLoader = sp.GetRequiredService<Infrastructure.Reports.Html.IReportResourceLoader>();
            var htmlBuilderFactory = new Func<Infrastructure.Reports.Html.IHtmlBuilder>(() =>
                sp.GetRequiredService<Infrastructure.Reports.Html.IHtmlBuilder>());
            return new Infrastructure.Reports.Html.ReportHtmlWrapper(resourceLoader, htmlBuilderFactory);
        });
        services.AddScoped<Infrastructure.Reports.Html.IReportTableGenerator>(sp =>
        {
            var htmlBuilderFactory = new Func<Infrastructure.Reports.Html.IHtmlBuilder>(() =>
                sp.GetRequiredService<Infrastructure.Reports.Html.IHtmlBuilder>());
            return new Infrastructure.Reports.Html.ReportTableGenerator(htmlBuilderFactory);
        });

        services.AddScoped<Infrastructure.Reports.IReportLocalReportFactory>(sp =>
        {
            var environment = sp.GetRequiredService<IWebHostEnvironment>();
            var persianDateTime = sp.GetRequiredService<PersianDateTime>();
            var reportsPath = Path.Combine(environment.WebRootPath, "Reports");
            return new Infrastructure.Reports.ReportLocalReportFactory(reportsPath, persianDateTime);
        });

        services.AddScoped<Infrastructure.Reports.IReportRenderer, Infrastructure.Reports.ReportRenderer>();
        services.AddScoped<IReportService, RdlcReportService>();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<ApplicationDbContext>(
            options => options
                .UseSqlServer(connectionString, sqlServerOptions =>
                    sqlServerOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default)));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddSqlServer(configuration.GetConnectionString("Database")!);

        return services;
    }

    private static IServiceCollection AddAuthenticationInternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!)),
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<Application.Abstractions.Authentication.ILibraryTokenProvider, Infrastructure.Authentication.LibraryTokenProvider>();

        return services;
    }

    private static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
    {
        services.AddAuthorization();

        services.AddScoped<PermissionProvider>();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationHandler, Authorization.RoleEndpointAuthorizationHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        services.AddScoped<Application.Abstractions.Authorization.IRolePermissionService, Authorization.RoleEndpointService>();

        return services;
    }
}
