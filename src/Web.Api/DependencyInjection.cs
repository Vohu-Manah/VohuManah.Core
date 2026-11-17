using Microsoft.AspNetCore.Http;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Web.Api.Middleware;

namespace VohuManah.Web.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddControllers();

        services.AddScoped<CsrfMiddleware>();

        services.AddCors(options =>
            options.AddPolicy("CorsPolicy", policy =>
                policy
                    .SetIsOriginAllowed(_ => true)
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .AllowAnyHeader()));

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
