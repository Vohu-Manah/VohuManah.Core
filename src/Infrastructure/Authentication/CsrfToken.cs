using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Authentication;

public static class CsrfToken
{
    public const string CookieName = "XSRF-TOKEN";
    public const string HeaderName = "X-XSRF-TOKEN";

    public static string CreateToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    public static void SetCsrfCookie(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Append(CookieName, CreateToken(), new CookieOptions
        {
            HttpOnly = false,
            Secure = AuthCookies.UseSecureCookies,
            SameSite = SameSiteMode.Lax,
            Path = "/"
        });
    }

    public static void ClearCsrfCookie(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete(CookieName);
    }
}
