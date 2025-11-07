using Application.Abstractions.Messaging;
using Application.Library.Users.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Users;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/users/{userName}", async (
            string userName,
            ICommandHandler<DeleteLibraryUserCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteLibraryUserCommand(userName);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .WithTags("Library.Users");
    }
}

