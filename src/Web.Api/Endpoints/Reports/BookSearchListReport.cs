using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;
using VohuManah.Application.Library.Books.Search;
using VohuManah.Application.Library.Settings;
using VohuManah.Infrastructure.Reports;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Endpoints.Reports.Requests;
using VohuManah.Web.Api.Infrastructure;

namespace VohuManah.Web.Api.Endpoints.Reports;

[RequireRole(EndpointNames.Reports.BookSearchList)]
internal sealed class BookSearchListReport : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("reports/book-search-list", async (
            BookSearchListReportRequest request,
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
                    request.ToPublishYear, request.MaxResults, request.Page, request.PageSize);

                var searchResult = await searchHandler.Handle(searchQuery, cancellationToken);

                if (searchResult.IsFailure)
                {
                    return CustomResults.Problem(
                        title: "خطا در جستجو",
                        detail: searchResult.Error.Description);
                }

                var books = searchResult.Value.Items;

                var parameters = new Dictionary<string, object>
                {
                    { "Today", ReportExportHelper.PersianToday() },
                    { "Page", request.Page },
                    { "PageSize", request.PageSize },
                    { "TotalCount", searchResult.Value.TotalCount },
                    { "TotalPages", (int)Math.Ceiling(searchResult.Value.TotalCount / (double)Math.Max(1, request.PageSize)) }
                };

                var htmlContent = await reportService.GenerateHtmlAsync(
                    ReportConstants.BookSearchListReport,
                    ReportConstants.DsBooks,
                    books,
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
