using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Books.Search;

public sealed record SearchBooksQuery(
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
    int PageSize = 50) : IQuery<PagedResult<BookSearchResponse>>;
