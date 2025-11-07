using Application.Abstractions.Messaging;
using Application.Library.Users.GetFullName;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Users;

internal sealed class GetFullName : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/users/{userName}/fullname", async (
            string userName,
            IQueryHandler<GetUserFullNameQuery, string?> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetUserFullNameQuery(userName);

            Result<string?> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Users");
    }
}

