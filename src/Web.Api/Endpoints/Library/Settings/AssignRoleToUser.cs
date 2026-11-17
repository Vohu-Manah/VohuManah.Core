using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings.AssignRoleToUser;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(PermissionNames.Settings.AssignRoleToUser)]
internal sealed class AssignRoleToUser : BaseEndpoint
{
    public sealed record Request(int RoleId);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/settings/users/{userId:long}/roles", async (
            long userId,
            Request request,
            ICommandHandler<AssignRoleToUserCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AssignRoleToUserCommand(userId, request.RoleId);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

