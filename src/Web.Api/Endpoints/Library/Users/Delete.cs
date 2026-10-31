using Application.Abstractions.Messaging;
using Application.Library.Users.Delete;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Users;

[RequireRole(EndpointNames.Users.Delete)]
internal sealed class Delete : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/users/{id:long}", async (
            long id,
            ICommandHandler<DeleteLibraryUserCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteLibraryUserCommand(id);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Users");
    }
}

