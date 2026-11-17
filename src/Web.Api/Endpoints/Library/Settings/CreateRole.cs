using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings.CreateRole;
using VohuManah.Application.Library.Settings.GetAllRoles;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(PermissionNames.Settings.CreateRole)]
internal sealed class CreateRole : BaseEndpoint
{
    public sealed record Request(string Name);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/settings/roles", async (
            Request request,
            ICommandHandler<CreateRoleCommand, RoleResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateRoleCommand(request.Name);

            Result<RoleResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

