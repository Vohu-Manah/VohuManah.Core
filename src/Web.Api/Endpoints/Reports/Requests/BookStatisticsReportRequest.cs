namespace Web.Api.Endpoints.Reports.Requests;

internal sealed record BookStatisticsReportRequest
{
    public int FilterType { get; init; } = 1;
}
