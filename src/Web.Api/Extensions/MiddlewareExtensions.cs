using Web.Api.Middleware;

namespace Web.Api.Extensions;

public static class MiddlewareExtensions
{
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
