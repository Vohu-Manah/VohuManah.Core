using System;
using Microsoft.AspNetCore.Http;

namespace VohuManah.Infrastructure.Authentication;

public static class AuthCookies
{
    public const string AccessTokenCookie = "access_token";
    public const string RefreshTokenCookie = "refresh_token";

    public static bool UseSecureCookies =>
        Environment.GetEnvironmentVariable("COOKIE_SECURE")?.Equals("1", StringComparison.OrdinalIgnoreCase) == true;

    public static void SetAuthCookies(HttpContext httpContext, string accessToken, string refreshToken)
    {
        var secure = UseSecureCookies;

        httpContext.Response.Cookies.Append(AccessTokenCookie, accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = secure,
            SameSite = SameSiteMode.Lax,
            Path = "/"
        });

        httpContext.Response.Cookies.Append(RefreshTokenCookie, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = secure,
            SameSite = SameSiteMode.Lax,
            Path = "/"
        });

        CsrfToken.SetCsrfCookie(httpContext);
    }

    public static void ClearAuthCookies(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete(AccessTokenCookie);
        httpContext.Response.Cookies.Delete(RefreshTokenCookie);
        CsrfToken.ClearCsrfCookie(httpContext);
    }

    public static string? GetAccessToken(HttpContext httpContext) =>
        httpContext.Request.Cookies.TryGetValue(AccessTokenCookie, out var token) ? token : null;

    public static string? GetRefreshToken(HttpContext httpContext) =>
        httpContext.Request.Cookies.TryGetValue(RefreshTokenCookie, out var token) ? token : null;
}
