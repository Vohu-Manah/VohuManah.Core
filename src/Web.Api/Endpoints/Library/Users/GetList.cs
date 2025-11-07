using Application.Abstractions.Messaging;
using Application.Library.Users.GetList;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Users;

internal sealed class GetList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/users/list", async (
            IQueryHandler<GetUserListQuery, List<UserListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserListQuery();

            Result<List<UserListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Users");
    }
}

