using Application.Abstractions.Messaging;
using Application.Library.Books.GetAllEntities;
using Domain.Library;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Books;

internal sealed class GetAllEntities : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/books/entities", async (
            IQueryHandler<GetAllBookEntitiesQuery, List<Book>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllBookEntitiesQuery();

            Result<List<Book>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Books");
    }
}

