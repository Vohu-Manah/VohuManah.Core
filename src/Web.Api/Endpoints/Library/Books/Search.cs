using Application.Abstractions.Messaging;
using Application.Library.Shared;
using Application.Library.Books.Search;
using SharedKernel;
using Web.Api.Endpoints.Attributes;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Books;

using Application.Library.Settings;

[RequireRole(EndpointNames.Books.Search)]
internal sealed class Search : BaseEndpoint
{
    public sealed record Request(
        string? Name,
        string? Author,
        string? Translator,
        string? Editor,
        string? Isbn,
        string? No,
        string? Corrector,
        int FromVolumeCount,
        int ToVolumeCount,
        int LanguageId,
        int SubjectId,
        int PublishPlaceId,
        int PublisherId,
        string? FromPublishYear,
        string? ToPublishYear,
        int MaxResults = 5000,
        int Page = 1,
        int PageSize = 50);

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/books/search", async (
            Request request,
            IQueryHandler<SearchBooksQuery, PagedResult<BookSearchResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new SearchBooksQuery(
                request.Name,
                request.Author,
                request.Translator,
                request.Editor,
                request.Isbn,
                request.No,
                request.Corrector,
                request.FromVolumeCount,
                request.ToVolumeCount,
                request.LanguageId,
                request.SubjectId,
                request.PublishPlaceId,
                request.PublisherId,
                request.FromPublishYear,
                request.ToPublishYear, request.MaxResults, request.Page, request.PageSize);

            Result<PagedResult<BookSearchResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Library.Books");
    }
}
