using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Application.Library.Books.Search;
using VohuManah.Application.Abstractions.Reports;
using Microsoft.AspNetCore.Mvc;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Endpoints.Reports.Requests;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Library.Settings;

namespace VohuManah.Web.Api.Endpoints.Reports;

[RequireRole(EndpointNames.Reports.Export)]
internal sealed class ExportBookSearchListReport : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("reports/export/book-search-list", async (
            BookSearchListReportRequest request,
            [FromQuery] string format,
            IQueryHandler<SearchBooksQuery, PagedResult<BookSearchResponse>> searchHandler,
            IReportService reportService,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var searchQuery = new SearchBooksQuery(
                    request.Name,
                    request.Author,
                    request.Translator,
                    request.Editor,
                    request.Isbn,
                    request.No,
                    request.Corrector,
                    request.FromVolumeCount,
                    request.ToVolumeCount,
                    request.LanguageId,
                    request.SubjectId,
                    request.PublishPlaceId,
                    request.PublisherId,
                    request.FromPublishYear,
                    request.ToPublishYear,
                    50000,
                    1,
                    50000);

                var searchResult = await searchHandler.Handle(searchQuery, cancellationToken);

                if (searchResult.IsFailure)
                {
                    return CustomResults.Problem(
                        title: "خطا در جستجو",
                        detail: searchResult.Error.Description);
                }

                var books = searchResult.Value.Items;

                return await ReportExportHelper.ExportReportAsync(
                    books,
                    ReportConstants.BookSearchListReport,
                    ReportConstants.DsBooks,
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

