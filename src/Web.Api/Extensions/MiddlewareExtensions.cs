using Microsoft.AspNetCore.Http.Features;
using VohuManah.Web.Api.Middleware;

namespace VohuManah.Web.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLimits(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/reports"))
                context.Response.Headers.CacheControl = "no-store";

            if (!context.Request.Path.StartsWithSegments("/library/backup/restore"))
            {
                var bodySizeFeature = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
                if (bodySizeFeature is not null)
                {
                    bodySizeFeature.MaxRequestBodySize = 500L * 1024 * 1024;
                }
            }

            await next();
        });

        return app;
    }

    public static IApplicationBuilder UseRequestContextLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestContextLoggingMiddleware>();

        return app;
    }

    public static IApplicationBuilder UseCookieTokenAuthentication(this IApplicationBuilder app)
    {
        app.UseMiddleware<CookieTokenAuthenticationMiddleware>();

        return app;
    }

    public static IApplicationBuilder UseCsrfProtection(this IApplicationBuilder app)
    {
        app.UseMiddleware<CsrfMiddleware>();

        return app;
    }
}
