using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Settings.GetUserRoles;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Settings;

[RequireRole(PermissionNames.Settings.GetUserRoles)]
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

