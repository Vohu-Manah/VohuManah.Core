using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using VohuManah.Web.Api.Security;

namespace VohuManah.Web.Api.Middleware;

public sealed class CsrfMiddleware : IMiddleware
{
    private static readonly HashSet<string> SafeMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        "GET",
        "HEAD",
        "OPTIONS",
        "TRACE"
    };

    private static readonly HashSet<string> ExemptPrefixes = new(StringComparer.Ordinal)
    {
        "/library/users/login",
        "/library/users/refresh-token",
        "/library/human-verify",
        "/library/attachments",
        "/library/backup/restore"
    };

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var isSafeMethod = SafeMethods.Contains(context.Request.Method);
        var isExempt = ExemptPrefixes.Any(p => context.Request.Path.StartsWithSegments(p));

        if (!isSafeMethod && !isExempt)
        {
            var cookieToken = context.Request.Cookies[CsrfToken.CookieName];
            var headerToken = context.Request.Headers[CsrfToken.HeaderName].FirstOrDefault();

            var valid = false;
            if (!string.IsNullOrEmpty(cookieToken) && !string.IsNullOrEmpty(headerToken))
            {
                try
                {
                    valid = CryptographicOperations.FixedTimeEquals(
                        Convert.FromBase64String(cookieToken),
                        Convert.FromBase64String(headerToken));
                }
                catch (FormatException)
                {
                    valid = false;
                }
            }

            if (!valid)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    type = "https://httpstatuses.com/403",
                    title = "توکن ضد جعل معتبر نیست",
                    status = StatusCodes.Status403Forbidden,
                    detail = "درخواست به دلیل نبود توکن CSRF معتبر رد شد."
                });
                return;
            }
        }

        await next(context);
    }
}
