using Application.Abstractions.Messaging;
using Application.Library.Books.GetList;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Books;

using Application.Library.Settings;

[RequireRole(EndpointNames.Books.GetList)]
internal sealed class GetList : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("library/books/list", async (
            [AsParameters] BookListRequest request,
            IQueryHandler<GetBookListQuery, PagedList<BookListResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBookListQuery
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Search = request.Search,
                Year = request.Year,
                FromDate = request.FromDate,
                ToDate = request.ToDate
            };

            Result<PagedList<BookListResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Books");
    }
}

internal sealed record BookListRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    
    public string? Search { get; init; }
    public string? Year { get; init; }
    public string? FromDate { get; init; }
    public string? ToDate { get; init; }
}
