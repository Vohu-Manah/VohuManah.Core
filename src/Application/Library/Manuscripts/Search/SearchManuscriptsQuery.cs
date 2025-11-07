using Application.Abstractions.Messaging;

namespace Application.Library.Manuscripts.Search;

public sealed record SearchManuscriptsQuery(
    string? Name,
    string? Author,
    int WritingPlaceId,
    string? FromYear,
    string? ToYear,
    int FromPageCount,
    int ToPageCount,
    string? No,
    int LanguageId,
    int SubjectId,
    int GapId,
    string? Size) : IQuery<List<ManuscriptSearchResponse>>;

