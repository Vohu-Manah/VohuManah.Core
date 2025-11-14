using Application.Abstractions.Messaging;
using Application.Library.Books.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Books;

[RequireRole("Library.Books.GetById")]
internal sealed class GetById : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/books/{id:long}", async (
            long id,
            IQueryHandler<GetBookByIdQuery, BookResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBookByIdQuery(id);

            Result<BookResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Books");
    }
}

