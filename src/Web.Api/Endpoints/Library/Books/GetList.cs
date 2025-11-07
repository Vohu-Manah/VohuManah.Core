using Application.Abstractions.Messaging;
using Application.Library.Books.GetList;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Books;

internal sealed class GetList : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/books/list", async (
            IQueryHandler<GetBookListQuery, List<BookListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBookListQuery();

            Result<List<BookListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Books");
    }
}

