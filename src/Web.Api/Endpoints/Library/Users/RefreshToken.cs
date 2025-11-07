using Application.Abstractions.Messaging;
using Application.Library.Users.Tokens;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Users;

internal sealed class RefreshToken : IEndpoint
{
    public sealed record Request(string RefreshToken);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/users/refresh-token", async (
            Request request,
            ICommandHandler<RefreshAccessTokenCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RefreshAccessTokenCommand(request.RefreshToken);
            Result<string> result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .AllowAnonymous()
        .WithTags("Library.Users");
    }
}


