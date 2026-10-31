namespace Web.Api.Endpoints.Reports.Requests;

internal sealed record PublicationStatisticsReportRequest
{
    public int FilterType { get; init; } = 1;
}
