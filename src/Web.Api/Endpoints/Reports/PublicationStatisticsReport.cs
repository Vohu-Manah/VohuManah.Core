using Application.Abstractions.Messaging;
using Application.Library.Publications.GetStatistics;
using Infrastructure.Reports;
using Web.Api.Endpoints.Attributes;
using Web.Api.Endpoints.Reports.Requests;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Reports;

[RequireRole(EndpointNames.Reports.PublicationStatistics)]
internal sealed class PublicationStatisticsReport : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("reports/publication-statistics", async (
            [AsParameters] PublicationStatisticsReportRequest request,
            IQueryHandler<GetPublicationStatisticsQuery, PublicationStatisticsResponse> handler,
            IReportService reportService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var query = new GetPublicationStatisticsQuery
                {
                    FilterType = request.FilterType
                };

                var result = await handler.Handle(query, cancellationToken);

                if (result.IsFailure)
                {
                    return CustomResults.Problem(
                        title: "Ø®Ø·Ø§ Ø¯Ø± Ø¯Ø±ÛŒØ§ÙØª Ø¯Ø§Ø¯Ù‡\u200CÙ‡Ø§",
                        detail: result.Error.Description);
                }

                var statistics = result.Value;

                var dataSource = request.FilterType switch
                {
                    1 => statistics.CountByType,
                    2 => statistics.CountByPublishPlace,
                    3 => statistics.CountByPublishDate,
                    4 => statistics.CountByLanguage,
                    5 => statistics.CountBySubject,
                    _ => statistics.CountByType
                };

                var filterTitles = new Dictionary<int, string>
                {
                    { 1, "نوع" },
                    { 2, "محل انتشار" },
                    { 3, "تاریخ انتشار" },
                    { 4, "زبان" },
                    { 5, "موضوع" }
                };

                var parameters = new Dictionary<string, object>
                {
                    { "Title", $"گزارش آماری براساس {filterTitles.GetValueOrDefault(request.FilterType, "Ù†ÙˆØ¹")}" },
                    { "PublicationCount", statistics.PublicationCount }
                };

                var htmlContent = await reportService.GenerateHtmlAsync(
                    ReportConstants.PublicationStatisticReport,
                    ReportConstants.DsPublicationStatistic,
                    dataSource,
                    parameters,
                    cancellationToken);

                return Results.Content(htmlContent, "text/html; charset=utf-8");
            }
            catch (Exception ex)
            {
                return CustomResults.Problem(
                    title: "خطا در تولید گزارش",
                    detail: ex.Message);
            }
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Reports");
    }
}
