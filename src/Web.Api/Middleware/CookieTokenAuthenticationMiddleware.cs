using Microsoft.AspNetCore.Http;
using VohuManah.Web.Api.Security;

namespace VohuManah.Web.Api.Middleware;

public sealed class CookieTokenAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public CookieTokenAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var accessToken = AuthCookies.GetAccessToken(context);

        if (!string.IsNullOrEmpty(accessToken) &&
            string.IsNullOrEmpty(context.Request.Headers.Authorization))
        {
            context.Request.Headers.Authorization = $"Bearer {accessToken}";
        }

        await _next(context);
    }
}
