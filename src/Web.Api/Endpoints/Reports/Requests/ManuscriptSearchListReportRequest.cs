namespace Web.Api.Endpoints.Reports.Requests;

internal sealed record ManuscriptSearchListReportRequest(
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
    int PageSize = 30);
