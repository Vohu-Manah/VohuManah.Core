using Application.Abstractions.Messaging;
using Application.Library.Users.GetAll;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Users;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/users", async (
            IQueryHandler<GetAllLibraryUsersQuery, List<LibraryUserResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllLibraryUsersQuery();

            Result<List<LibraryUserResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Users");
    }
}
