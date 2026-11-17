using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Abstractions.Authorization;
using VohuManah.Application.Library.Settings.GetRolePermissions;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(PermissionNames.Settings.GetRolePermissions)]
internal sealed class GetRolePermissions : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings/roles/{roleId:int}/permissions", async (
            int roleId,
            IQueryHandler<GetRolePermissionsQuery, List<string>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetRolePermissionsQuery(roleId);

            Result<List<string>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

