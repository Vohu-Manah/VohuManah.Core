using Application.Abstractions.Messaging;
using Application.Library.Users.Update;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Users;

internal sealed class Update : IEndpoint
{
    public sealed record Request(
        string UserName,
        string Password,
        string Name,
        string LastName);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/users", async (
            Request request,
            ICommandHandler<UpdateLibraryUserCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateLibraryUserCommand(
                request.UserName,
                request.Password,
                request.Name,
                request.LastName);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .WithTags("Library.Users");
    }
}

