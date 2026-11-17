using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings.GetUserRoleIds;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(PermissionNames.Settings.GetUserRoles)]
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

