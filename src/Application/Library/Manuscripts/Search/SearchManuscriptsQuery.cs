using Application.Abstractions.Messaging;
using Application.Library.Shared;

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
    string? Size,
    int MaxResults = 50000,
    int Page = 1,
    int PageSize = 50) : IQuery<PagedResult<ManuscriptSearchResponse>>;

