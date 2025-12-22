using Application.Abstractions.Messaging;
using Application.Library.Settings.GetRolePermissions;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Settings;

[RequireRole("Library.Settings.GetAllRoles")]
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

