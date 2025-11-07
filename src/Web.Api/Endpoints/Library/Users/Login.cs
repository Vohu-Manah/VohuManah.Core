using Application.Abstractions.Messaging;
using Application.Library.Users.Login;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Users;

internal sealed class Login : IEndpoint
{
    public sealed record Request(string UserName, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/users/login", async (
            Request request,
            ICommandHandler<LoginLibraryUserCommand, LoginLibraryUserResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginLibraryUserCommand(request.UserName, request.Password);

            Result<LoginLibraryUserResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .AllowAnonymous()
        .WithTags("Library.Users");
    }
}
