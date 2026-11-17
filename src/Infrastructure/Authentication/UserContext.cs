using System;
using VohuManah.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace VohuManah.Infrastructure.Authentication;

internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long UserId =>
        _httpContextAccessor
            .HttpContext?
            .User
            .GetUserId() ??
        throw new InvalidOperationException("کانتکست کاربر در دسترس نیست");
}
