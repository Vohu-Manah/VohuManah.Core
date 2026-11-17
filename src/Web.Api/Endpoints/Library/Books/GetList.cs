using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Books.GetList;
using VohuManah.SharedKernel;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Extensions;
using VohuManah.Web.Api.Http;

namespace VohuManah.Web.Api.Endpoints.Library.Books;

using VohuManah.Application.Abstractions.Authorization;

[RequireRole(PermissionNames.Books.GetList)]
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
