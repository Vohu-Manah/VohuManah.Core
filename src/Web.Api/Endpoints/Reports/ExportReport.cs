using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Books.GetStatistics;
using VohuManah.Application.Library.Manuscripts.GetStatistics;
using VohuManah.Application.Library.Publications.GetStatistics;
using VohuManah.Application.Abstractions.Reports;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using VohuManah.Web.Api.Endpoints.Attributes;
using VohuManah.Web.Api.Infrastructure;
using VohuManah.Application.Abstractions.Authorization;

namespace VohuManah.Web.Api.Endpoints.Reports;

[RequireRole(PermissionNames.Reports.Export)]
internal sealed class ExportReport : BaseEndpoint
{
    private static readonly Dictionary<string, string[]> StatFilterTitles = new()
    {
        [ReportConstants.BookStatisticReport] = new[] { "ناشر", "محل انتشار", "سال انتشار", "زبان", "موضوع" },
        [ReportConstants.ManuscriptStatisticReport] = new[] { "فاصله", "محل نگارش", "سال", "زبان", "موضوع" },
        [ReportConstants.PublicationStatisticReport] = new[] { "نوع", "محل انتشار", "تاریخ انتشار", "زبان", "موضوع" },
    };

    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("reports/export/{reportName}", async (
            string reportName,
            [FromQuery] string format,
            [FromQuery] string datasourceName,
            [FromQuery] string? parameters,
            [FromQuery] int? filterType,
            IReportService reportService,
            IQueryHandler<GetBookStatisticsQuery, BookStatisticsResponse>? bookStatsHandler,
            IQueryHandler<GetManuscriptStatisticsQuery, ManuscriptStatisticsResponse>? manuscriptStatsHandler,
            IQueryHandler<GetPublicationStatisticsQuery, PublicationStatisticsResponse>? publicationStatsHandler,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var paramDict = ReportParameterParser.Parse(parameters);
                    filterType ??= 1;
                IEnumerable dataSource;

                if (reportName == ReportConstants.BookStatisticReport && filterType.HasValue)
                {
                    if (bookStatsHandler is null)
                    {
                        return CustomResults.Problem(
                            title: "خطا در تولید گزارش",
                            detail: "Handler برای آمار کتاب ها یافت نشد",
                            statusCode: StatusCodes.Status500InternalServerError);
                    }

                    var query = new GetBookStatisticsQuery { FilterType = filterType.Value };
                    var result = await bookStatsHandler.Handle(query, cancellationToken);

                    if (result.IsFailure)
                    {
                        return CustomResults.Problem(
                            title: "خطا در دریافت داده ها",
                            detail: result.Error.Description);
                    }

                    var statistics = result.Value;
                    dataSource = filterType.Value switch
                    {
                        1 => statistics.CountByPublisher,
                        2 => statistics.CountByPublishPlace,
                        3 => statistics.CountByYear,
                        4 => statistics.CountByLanguage,
                        5 => statistics.CountBySubject,
                        _ => statistics.CountByPublisher
                    };

                    paramDict["BookCount"] = statistics.BookCount;
                    paramDict["VolumeCount"] = statistics.VolumeCount;
                    paramDict["Title"] = $"گزارش آماری براساس {StatFilterTitles[ReportConstants.BookStatisticReport][filterType.Value - 1]}";
                    datasourceName = ReportConstants.DsBookStatistic;
                }
                else if (reportName == ReportConstants.ManuscriptStatisticReport && filterType.HasValue)
                {
                    if (manuscriptStatsHandler is null)
                    {
                        return CustomResults.Problem(
                            title: "خطا در تولید گزارش",
                            detail: "Handler برای آمار نسخ خطی یافت نشد",
                            statusCode: StatusCodes.Status500InternalServerError);
                    }

                    var query = new GetManuscriptStatisticsQuery { FilterType = filterType.Value };
                    var result = await manuscriptStatsHandler.Handle(query, cancellationToken);

                    if (result.IsFailure)
                    {
                        return CustomResults.Problem(
                            title: "خطا در دریافت داده ها",
                            detail: result.Error.Description);
                    }

                    var statistics = result.Value;
                    dataSource = filterType.Value switch
                    {
                        1 => statistics.CountByGap,
                        2 => statistics.CountByWritingPlace,
                        3 => statistics.CountByYear,
                        4 => statistics.CountByLanguage,
                        5 => statistics.CountBySubject,
                        _ => statistics.CountByGap
                    };

                    paramDict["ManuscriptCount"] = statistics.ManuscriptCount;
                    paramDict["Title"] = $"گزارش آماری براساس {StatFilterTitles[ReportConstants.ManuscriptStatisticReport][filterType.Value - 1]}";
                    datasourceName = ReportConstants.DsManuscriptStatistic;
                }
                else if (reportName == ReportConstants.PublicationStatisticReport && filterType.HasValue)
                {
                    if (publicationStatsHandler is null)
                    {
                        return CustomResults.Problem(
                            title: "خطا در تولید گزارش",
                            detail: "Handler برای آمار نشریات یافت نشد",
                            statusCode: StatusCodes.Status500InternalServerError);
                    }

                    var query = new GetPublicationStatisticsQuery { FilterType = filterType.Value };
                    var result = await publicationStatsHandler.Handle(query, cancellationToken);

                    if (result.IsFailure)
                    {
                        return CustomResults.Problem(
                            title: "خطا در دریافت داده ها",
                            detail: result.Error.Description);
                    }

                    var statistics = result.Value;
                    dataSource = filterType.Value switch
                    {
                        1 => statistics.CountByType,
                        2 => statistics.CountByPublishPlace,
                        3 => statistics.CountByPublishDate,
                        4 => statistics.CountByLanguage,
                        5 => statistics.CountBySubject,
                        _ => statistics.CountByType
                    };

                    paramDict["PublicationCount"] = statistics.PublicationCount;
                    paramDict["Title"] = $"گزارش آماری براساس {StatFilterTitles[ReportConstants.PublicationStatisticReport][filterType.Value - 1]}";
                    datasourceName = ReportConstants.DsPublicationStatistic;
                }
                else
                {
                    dataSource = Array.Empty<object>();
                }

                byte[] fileBytes;
                string contentType;
                string fileName;

                switch (format.ToUpperInvariant())
                {
                    case "PDF":
                        fileBytes = await reportService.GeneratePdfAsync(
                            reportName,
                            datasourceName,
                            dataSource,
                            paramDict,
                            cancellationToken);
                        contentType = "application/pdf";
                        fileName = $"{reportName}.pdf";
                        break;

                    case "EXCEL":
                    case "XLSX":
                        fileBytes = await reportService.GenerateExcelAsync(
                            reportName,
                            datasourceName,
                            dataSource,
                            paramDict,
                            cancellationToken);
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        fileName = $"{reportName}.xlsx";
                        break;

                    default:
                        return CustomResults.Problem(
                            title: "فرمت نامعتبر",
                            detail: $"فرمت {format} پشتیبانی نمیشود. فرمت های مجاز: pdf, excel",
                            statusCode: StatusCodes.Status400BadRequest);
                }

                return Results.File(fileBytes, contentType, fileName);
            }
            catch (NotSupportedException ex)
            {
                return CustomResults.Problem(
                    title: "PDF Export در دسترس نیست",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status501NotImplemented);
            }
            catch (FileNotFoundException ex)
            {
                return CustomResults.Problem(
                    title: "گزارش یافت نشد",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status404NotFound);
            }
            catch (Exception ex)
            {
                return CustomResults.Problem(
                    title: "خطا در تولید گزارش",
                    detail: $"Failed to generate {format.ToUpperInvariant()} report '{reportName}'. {ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .ApplyRoleAuthorization(this)
        .WithTags("Reports");
    }
}

