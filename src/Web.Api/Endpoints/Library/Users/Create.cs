using Application.Abstractions.Messaging;
using Application.Library.Users.Create;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Users;

internal sealed class Create : IEndpoint
{
    public sealed record Request(
        string UserName,
        string Password,
        string Name,
        string LastName);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/users", async (
            Request request,
            ICommandHandler<CreateLibraryUserCommand, string> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateLibraryUserCommand(
                request.UserName,
                request.Password,
                request.Name,
                request.LastName);

            Result<string> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Users");
    }
}
