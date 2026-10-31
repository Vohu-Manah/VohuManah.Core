using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Publications.Search;

public sealed record SearchPublicationsQuery(
    string? Name,
    int PublicationTypeId,
    string? Concessionaire,
    string? ResponsibleDirector,
    string? Editor,
    string? FromYear,
    string? ToYear,
    string? No,
    string? PublishDate,
    int PublishPlaceId,
    string? JournalNo,
    int LanguageId,
    int SubjectId,
    string? FromPeriod,
    string? ToPeriod,
    int MaxResults = 5000,
    int Page = 1,
    int PageSize = 50) : IQuery<PagedResult<PublicationSearchResponse>>;

