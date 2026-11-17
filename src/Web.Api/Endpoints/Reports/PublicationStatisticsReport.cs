using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Publications.GetStatistics;
using VohuManah.Infrastructure.Reports;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Endpoints.Reports.Requests;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Reports;

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
                        title: "خطا در دریافت داده ها",
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
                    { "Title", $"گزارش آماری براساس {filterTitles.GetValueOrDefault(request.FilterType, "نوع")}" },
                    { "PublicationCount", statistics.PublicationCount },
                    { "Today", ReportExportHelper.PersianToday() }
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
