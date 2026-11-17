using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Manuscripts.Search;
using VohuManah.Application.Abstractions.Reports;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Endpoints.Reports.Requests;
using VohuManah.Web.Api.Http;
using VohuManah.Application.Abstractions.Authorization;
using VohuManah.SharedKernel;

namespace VohuManah.Web.Api.Endpoints.Reports;

[RequireRole(PermissionNames.Reports.ManuscriptSearchList)]
internal sealed class ManuscriptSearchListReport : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("reports/manuscript-search-list", async (
            ManuscriptSearchListReportRequest request,
            IQueryHandler<SearchManuscriptsQuery, PagedList<ManuscriptSearchResponse>> searchHandler,
            IReportService reportService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var searchQuery = new SearchManuscriptsQuery(
                    request.Name,
                    request.Author,
                    request.WritingPlaceId,
                    request.FromYear,
                    request.ToYear,
                    request.FromPageCount,
                    request.ToPageCount,
                    request.No,
                    request.LanguageId,
                    request.SubjectId,
                    request.GapId,
                    request.Size, request.MaxResults, request.Page, request.PageSize);

                var searchResult = await searchHandler.Handle(searchQuery, cancellationToken);

                if (searchResult.IsFailure)
                {
                    return CustomResults.Problem(
                        title: "خطا در جستجو",
                        detail: searchResult.Error.Description);
                }

                var manuscripts = searchResult.Value.Items;

                var parameters = new Dictionary<string, object>
                {
                    { "Today", ReportExportHelper.PersianToday() },
                    { "Page", request.Page },
                    { "PageSize", request.PageSize },
                    { "TotalCount", searchResult.Value.TotalCount },
                    { "TotalPages", (int)Math.Ceiling(searchResult.Value.TotalCount / (double)Math.Max(1, request.PageSize)) }
                };

                var htmlContent = await reportService.GenerateHtmlAsync(
                    ReportConstants.ManuscriptSearchListReport,
                    ReportConstants.DsManuscripts,
                    manuscripts,
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