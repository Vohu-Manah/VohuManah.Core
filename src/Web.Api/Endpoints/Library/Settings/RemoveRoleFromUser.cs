using Application.Abstractions.Messaging;
using Application.Library.Settings.RemoveRoleFromUser;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Settings;

[RequireRole(EndpointNames.Settings.RemoveRoleFromUser)]
internal sealed class RemoveRoleFromUser : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/settings/users/{userId:long}/roles/{roleId:int}", async (
            long userId,
            int roleId,
            ICommandHandler<RemoveRoleFromUserCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RemoveRoleFromUserCommand(userId, roleId);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

