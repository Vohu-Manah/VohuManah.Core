using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Abstractions.Authorization;
using VohuManah.Application.Library.Settings.GetAllRoles;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(PermissionNames.Settings.GetAllRoles)]
internal sealed class GetAllRoles : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings/roles", async (
            IQueryHandler<GetAllRolesQuery, List<RoleResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllRolesQuery();

            Result<List<RoleResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

