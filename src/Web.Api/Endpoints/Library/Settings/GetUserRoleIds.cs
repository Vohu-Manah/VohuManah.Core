using Application.Abstractions.Messaging;
using Application.Library.Settings.GetUserRoleIds;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Settings;

[RequireRole(EndpointNames.Settings.GetUserRoles)]
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

