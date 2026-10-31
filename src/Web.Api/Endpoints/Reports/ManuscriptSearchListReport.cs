using Application.Abstractions.Messaging;
using Application.Library.Shared;
using Application.Library.Manuscripts.Search;
using Infrastructure.Reports;
using Web.Api.Endpoints.Attributes;
using Web.Api.Endpoints.Reports.Requests;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Reports;

[RequireRole(EndpointNames.Reports.ManuscriptSearchList)]
internal sealed class ManuscriptSearchListReport : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("reports/manuscript-search-list", async (
            ManuscriptSearchListReportRequest request,
            IQueryHandler<SearchManuscriptsQuery, PagedResult<ManuscriptSearchResponse>> searchHandler,
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

                if (manuscripts.Count == 0)
                {
                    return CustomResults.Problem(
                        title: "گزارش خالی",
                        detail: "گزارش درخواستی خالی می باشد",
                        statusCode: StatusCodes.Status400BadRequest);
                }

                var parameters = new Dictionary<string, object>
                {
                    { "Today", DateTime.Now.ToString("yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture) },
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
