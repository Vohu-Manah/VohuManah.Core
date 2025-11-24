using System.Security.Claims;

namespace Infrastructure.Authentication;

internal static class ClaimsPrincipalExtensions
{
    public static long GetUserId(this ClaimsPrincipal? principal)
    {
        string? userId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        return long.TryParse(userId, out long parsedUserId) ?
            parsedUserId :
            throw new ApplicationException("User id is unavailable");
    }
}
