using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Application.Library.Manuscripts.Search;
using VohuManah.Application.Abstractions.Reports;
using Microsoft.AspNetCore.Mvc;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Endpoints.Reports.Requests;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Reports;

[RequireRole(PermissionNames.Reports.Export)]
internal sealed class ExportManuscriptSearchListReport : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("reports/export/manuscript-search-list", async (
            ManuscriptSearchListReportRequest request,
            [FromQuery] string format,
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
                    request.Size);

                var searchResult = await searchHandler.Handle(searchQuery, cancellationToken);

                if (searchResult.IsFailure)
                {
                    return CustomResults.Problem(
                        title: "خطا در جستجو",
                        detail: searchResult.Error.Description);
                }

                var manuscripts = searchResult.Value.Items;

                return await ReportExportHelper.ExportReportAsync(
                    manuscripts,
                    ReportConstants.ManuscriptSearchListReport,
                    ReportConstants.DsManuscripts,
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

