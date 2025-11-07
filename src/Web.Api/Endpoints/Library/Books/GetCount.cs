using Application.Abstractions.Messaging;
using Application.Library.Books.GetCount;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Books;

internal sealed class GetCount : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/books/count", async (
            IQueryHandler<GetBookCountQuery, int> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBookCountQuery();

            Result<int> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Books");
    }
}

