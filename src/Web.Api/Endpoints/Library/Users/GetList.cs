using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Users.GetList;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Library.Users;

[RequireRole(PermissionNames.Users.GetList)]
internal sealed class GetList : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/users/list", async (
            IQueryHandler<GetUserListQuery, List<UserListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserListQuery();

            Result<List<UserListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Users");
    }
}

