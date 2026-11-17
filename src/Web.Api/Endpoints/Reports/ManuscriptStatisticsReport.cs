using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Manuscripts.GetStatistics;
using VohuManah.Application.Abstractions.Reports;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Endpoints.Reports.Requests;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Reports;

[RequireRole(EndpointNames.Reports.ManuscriptStatistics)]
internal sealed class ManuscriptStatisticsReport : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("reports/manuscript-statistics", async (
            [AsParameters] ManuscriptStatisticsReportRequest request,
            IQueryHandler<GetManuscriptStatisticsQuery, ManuscriptStatisticsResponse> handler,
            IReportService reportService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var query = new GetManuscriptStatisticsQuery
                {
                    FilterType = request.FilterType
                };

                var result = await handler.Handle(query, cancellationToken);

                if (result.IsFailure)
                {
                    return CustomResults.Problem(
                        title: "خطا در دریافت داده ها",
                        detail: result.Error.Description);
                }

                var statistics = result.Value;

                var dataSource = request.FilterType switch
                {
                    1 => statistics.CountByGap,
                    2 => statistics.CountByWritingPlace,
                    3 => statistics.CountByYear,
                    4 => statistics.CountByLanguage,
                    5 => statistics.CountBySubject,
                    _ => statistics.CountByGap
                };

                var filterTitles = new Dictionary<int, string>
                {
                    { 1, "فاصله" },
                    { 2, "محل نگارش" },
                    { 3, "سال" },
                    { 4, "زبان" },
                    { 5, "موضوع" }
                };

                var parameters = new Dictionary<string, object>
                {
                    { "Title", $"گزارش آماری براساس {filterTitles.GetValueOrDefault(request.FilterType, "فاصله")}" },
                    { "ManuscriptCount", statistics.ManuscriptCount },
                    { "Today", ReportExportHelper.PersianToday() }
                };

                var htmlContent = await reportService.GenerateHtmlAsync(
                    ReportConstants.ManuscriptStatisticReport,
                    ReportConstants.DsManuscriptStatistic,
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
