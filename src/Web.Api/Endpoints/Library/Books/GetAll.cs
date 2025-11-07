using Application.Abstractions.Messaging;
using Application.Library.Books.GetAll;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Books;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/books", async (
            IQueryHandler<GetAllBooksQuery, List<BookResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllBooksQuery();

            Result<List<BookResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Books");
    }
}
