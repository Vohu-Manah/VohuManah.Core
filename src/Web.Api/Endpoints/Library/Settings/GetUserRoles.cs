using Application.Abstractions.Messaging;
using Application.Library.Settings.GetUserRoles;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Settings;

[RequireRole("Library.Settings.GetUserRoles")]
internal sealed class GetUserRoles : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings/users/{userId:long}/roles", async (
            long userId,
            IQueryHandler<GetUserRolesQuery, List<UserRoleResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserRolesQuery(userId);

            Result<List<UserRoleResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

