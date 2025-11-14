using Application.Abstractions.Messaging;
using Application.Library.Users.Delete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Users;

[RequireRole("Library.Users.Delete")]
internal sealed class Delete : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
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
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Users");
    }
}

