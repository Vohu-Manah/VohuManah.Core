using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings.UpdateRolePermissions;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(EndpointNames.Settings.UpdateRoleEndpoints)]
internal sealed class UpdateRoleEndpoints : BaseEndpoint
{
    public sealed record Request(List<string>? EndpointNames);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("library/settings/roles/{roleId:int}/endpoints", async (
            int roleId,
            Request request,
            ICommandHandler<UpdateRolePermissionsCommand> handler,
            CancellationToken cancellationToken) =>
        {
            IReadOnlyCollection<string> endpointNames = request.EndpointNames ?? new List<string>();

            var command = new UpdateRolePermissionsCommand(roleId, endpointNames);

            Result result = await handler.Handle(command, cancellationToken);

            return result.IsSuccess ? Results.Ok() : CustomResults.Problem(result);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

