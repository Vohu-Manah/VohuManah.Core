using Application.Abstractions.Messaging;
using Application.Library.Books.Search;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Library.Books;

internal sealed class Search : IEndpoint
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
        string? ToPublishYear);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("library/books/search", async (
            Request request,
            IQueryHandler<SearchBooksQuery, List<BookSearchResponse>> handler,
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
                request.ToPublishYear);

            Result<List<BookSearchResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Library.Books");
    }
}
