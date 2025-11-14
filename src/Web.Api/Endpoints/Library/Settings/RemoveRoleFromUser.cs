using Application.Abstractions.Messaging;
using Application.Library.Settings.RemoveRoleFromUser;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Settings;

[RequireRole("Library.Settings.RemoveRoleFromUser")]
internal sealed class RemoveRoleFromUser : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/settings/users/{userName}/roles/{roleId:int}", async (
            string userName,
            int roleId,
            ICommandHandler<RemoveRoleFromUserCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RemoveRoleFromUserCommand(userName, roleId);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

