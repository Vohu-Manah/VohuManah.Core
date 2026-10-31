namespace Web.Api.Endpoints.Reports.Requests;

internal sealed record PublicationSearchListReportRequest(
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
    int MaxResults = 50000,
    int Page = 1,
    int PageSize = 30);
