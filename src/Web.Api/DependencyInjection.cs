using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using VohuManah.Web.Api.Authorization;
using VohuManah.Web.Api.Http;
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

        services.AddTransient<IAuthorizationHandler, RoleEndpointAuthorizationHandler>();

        return services;
    }
}
