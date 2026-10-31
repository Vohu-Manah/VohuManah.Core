using Application.Abstractions.Messaging;
using Application.Library.Shared;
using Application.Library.Publications.Search;
using Infrastructure.Reports;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Endpoints.Attributes;
using Web.Api.Endpoints.Reports.Requests;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Reports;

[RequireRole(EndpointNames.Reports.Export)]
internal sealed class ExportPublicationSearchListReport : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("reports/export/publication-search-list", async (
            PublicationSearchListReportRequest request,
            [FromQuery] string format,
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
                    request.ToPeriod);

                var searchResult = await searchHandler.Handle(searchQuery, cancellationToken);

                if (searchResult.IsFailure)
                {
                    return CustomResults.Problem(
                        title: "خطا در جستجو",
                        detail: searchResult.Error.Description);
                }

                var publications = searchResult.Value.Items;

                return await ReportExportHelper.ExportReportAsync(
                    publications,
                    ReportConstants.PublicationSearchListReport,
                    ReportConstants.DsPublications,
                    format,
                    reportService,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                return ReportExportHelper.HandleExportException(ex, format);
            }
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Reports");
    }
}

