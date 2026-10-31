using Application.Abstractions.Messaging;
using Application.Library.Users.GetList;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Library.Users;

[RequireRole(EndpointNames.Users.GetList)]
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

