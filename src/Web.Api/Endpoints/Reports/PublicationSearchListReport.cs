using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Application.Library.Publications.Search;
using VohuManah.Infrastructure.Reports;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Endpoints.Reports.Requests;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Reports;

[RequireRole(EndpointNames.Reports.PublicationSearchList)]
internal sealed class PublicationSearchListReport : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("reports/publication-search-list", async (
            PublicationSearchListReportRequest request,
            IQueryHandler<SearchPublicationsQuery, PagedResult<PublicationSearchResponse>> searchHandler,
            IReportService reportService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var searchQuery = new SearchPublicationsQuery(
                    request.Name,
                    request.PublicationTypeId,
                    request.Concessionaire,
                    request.ResponsibleDirector,
                    request.Editor,
                    request.FromYear,
                    request.ToYear,
                    request.No,
                    request.PublishDate,
                    request.PublishPlaceId,
                    request.JournalNo,
                    request.LanguageId,
                    request.SubjectId,
                    request.FromPeriod,
                    request.ToPeriod, request.MaxResults, request.Page, request.PageSize);

                var searchResult = await searchHandler.Handle(searchQuery, cancellationToken);

                if (searchResult.IsFailure)
                {
                    return CustomResults.Problem(
                        title: "خطا در جستجو",
                        detail: searchResult.Error.Description);
                }

                var publications = searchResult.Value.Items;

                var parameters = new Dictionary<string, object>
                {
                    { "Today", ReportExportHelper.PersianToday() },
                    { "Page", request.Page },
                    { "PageSize", request.PageSize },
                    { "TotalCount", searchResult.Value.TotalCount },
                    { "TotalPages", (int)Math.Ceiling(searchResult.Value.TotalCount / (double)Math.Max(1, request.PageSize)) }
                };

                var htmlContent = await reportService.GenerateHtmlAsync(
                    ReportConstants.PublicationSearchListReport,
                    ReportConstants.DsPublications,
                    publications,
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
