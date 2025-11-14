using Application.Abstractions.Messaging;
using Application.Library.Books.GetList;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Endpoints;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Books;

[RequireRole("Library.Books.GetList")]
internal sealed class GetList : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/books/list", async (
            IQueryHandler<GetBookListQuery, List<BookListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBookListQuery();

            Result<List<BookListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Books");
    }
}

