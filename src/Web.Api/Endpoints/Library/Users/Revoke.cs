using Application.Abstractions.Messaging;
using Application.Library.Users.Tokens;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Users;

[RequireRole(EndpointNames.Users.Revoke)]
internal sealed class Revoke : BaseEndpoint
{
    public sealed record Request(string RefreshToken);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/users/revoke-token", async (
            Request request,
            ICommandHandler<RevokeRefreshTokenCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RevokeRefreshTokenCommand(request.RefreshToken);
            var result = await handler.Handle(command, cancellationToken);
            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Users");
    }
}


