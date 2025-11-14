using Web.Api.Extensions;

namespace Web.Api.Endpoints;

/// <summary>
/// Base class برای endpointها که به صورت خودکار نقش‌ها را از RequireRoleAttribute اعمال می‌کند
/// </summary>
public abstract class BaseEndpoint : IEndpoint
{
    public abstract void MapEndpoint(IEndpointRouteBuilder app);

    /// <summary>
    /// Helper method برای اعمال خودکار نقش‌ها از attribute
    /// این method از extension method ApplyRoleAuthorization استفاده می‌کند
    /// </summary>
    protected RouteHandlerBuilder ApplyRoleAuthorization(RouteHandlerBuilder builder)
    {
        return builder.ApplyRoleAuthorization(this);
    }
}

/// <summary>
/// Extension methods برای BaseEndpoint
/// </summary>
public static class BaseEndpointExtensions
{
    /// <summary>
    /// اعمال خودکار نقش‌ها از RequireRoleAttribute
    /// </summary>
    public static RouteHandlerBuilder ApplyRoleAuthorization(this RouteHandlerBuilder builder, BaseEndpoint endpoint)
    {
        return builder.ApplyRoleAuthorization((IEndpoint)endpoint);
    }
}

