using Application.Abstractions.Messaging;
using Application.Library.Users.Tokens;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Users;

internal sealed class Revoke : IEndpoint
{
    public sealed record Request(string RefreshToken);

    public void MapEndpoint(IEndpointRouteBuilder app)
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
        .WithTags("Library.Users");
    }
}


