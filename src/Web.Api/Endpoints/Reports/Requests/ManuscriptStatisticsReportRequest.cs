namespace Web.Api.Endpoints.Reports.Requests;

internal sealed record ManuscriptStatisticsReportRequest
{
    public int FilterType { get; init; } = 1;
}
