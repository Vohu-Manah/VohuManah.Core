namespace Web.Api.Endpoints.Reports.Requests;

internal sealed record BookSearchListReportRequest(
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
    int MaxResults = 50000,
    int Page = 1,
    int PageSize = 30);
