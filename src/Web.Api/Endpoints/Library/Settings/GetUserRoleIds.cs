using Application.Abstractions.Messaging;
using Application.Library.Settings.GetUserRoleIds;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Settings;

[RequireRole("Library.Settings.GetUserRoles")]
internal sealed class GetUserRoleIds : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/settings/users/{userId:long}/role-ids", async (
            long userId,
            IQueryHandler<GetUserRoleIdsQuery, List<int>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserRoleIdsQuery(userId);

            Result<List<int>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Settings");
    }
}

