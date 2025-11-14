using Application.Abstractions.Messaging;
using Application.Library.Settings.GetAllRoles;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Settings;

[RequireRole("Library.Settings.GetAllRoles")]
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

