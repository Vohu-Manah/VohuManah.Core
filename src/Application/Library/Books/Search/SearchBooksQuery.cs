using Application.Abstractions.Messaging;

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
    string? ToPublishYear) : IQuery<List<BookSearchResponse>>;
