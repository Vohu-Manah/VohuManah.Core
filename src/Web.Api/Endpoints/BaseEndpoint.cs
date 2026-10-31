using Web.Api.Extensions;

namespace Web.Api.Endpoints;

public abstract class BaseEndpoint : IEndpoint
{
    public abstract void MapEndpoint(IEndpointRouteBuilder app);


    protected RouteHandlerBuilder ApplyRoleAuthorization(RouteHandlerBuilder builder)
    {
        return builder.ApplyRoleAuthorization(this);
    }
}


public static class BaseEndpointExtensions
{
    public static RouteHandlerBuilder ApplyRoleAuthorization(this RouteHandlerBuilder builder, BaseEndpoint endpoint)
    {
        return builder.ApplyRoleAuthorization((IEndpoint)endpoint);
    }
}

