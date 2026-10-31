using Application.Abstractions.Messaging;
using Application.Library.Books.GetStatistics;
using Application.Library.Manuscripts.GetStatistics;
using Application.Library.Publications.GetStatistics;
using Infrastructure.Reports;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using Web.Api.Endpoints.Attributes;
using Web.Api.Infrastructure;
using Application.Library.Settings;

namespace Web.Api.Endpoints.Reports;

[RequireRole(EndpointNames.Reports.Export)]
internal sealed class ExportReport : BaseEndpoint
{
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
                IEnumerable dataSource;

                if (reportName == ReportConstants.BookStatisticReport && filterType.HasValue)
                {
                    if (bookStatsHandler is null)
                    {
                        return CustomResults.Problem(
                            title: "خطا در تولید گزارش",
                            detail: "Handler Ø¨Ø±Ø§ÛŒ Ø¢Ù…Ø§Ø± Ú©ØªØ§Ø¨\u200CÙ‡Ø§ ÛŒØ§ÙØª Ù†Ø´Ø¯",
                            statusCode: StatusCodes.Status500InternalServerError);
                    }

                    var query = new GetBookStatisticsQuery { FilterType = filterType.Value };
                    var result = await bookStatsHandler.Handle(query, cancellationToken);

                    if (result.IsFailure)
                    {
                        return CustomResults.Problem(
                            title: "Ø®Ø·Ø§ Ø¯Ø± Ø¯Ø±ÛŒØ§ÙØª Ø¯Ø§Ø¯Ù‡ Ù‡Ø§",
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
                }
                else if (reportName == ReportConstants.ManuscriptStatisticReport && filterType.HasValue)
                {
                    if (manuscriptStatsHandler is null)
                    {
                        return CustomResults.Problem(
                            title: "خطا در تولید گزارش",
                            detail: "Handler Ø¨Ø±Ø§ÛŒ Ø¢Ù…Ø§Ø± Ù†Ø³Ø® Ø®Ø·ÛŒ ÛŒØ§ÙØª Ù†Ø´Ø¯",
                            statusCode: StatusCodes.Status500InternalServerError);
                    }

                    var query = new GetManuscriptStatisticsQuery { FilterType = filterType.Value };
                    var result = await manuscriptStatsHandler.Handle(query, cancellationToken);

                    if (result.IsFailure)
                    {
                        return CustomResults.Problem(
                            title: "Ø®Ø·Ø§ Ø¯Ø± Ø¯Ø±ÛŒØ§ÙØª Ø¯Ø§Ø¯Ù‡\u200CÙ‡Ø§",
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
                }
                else if (reportName == ReportConstants.PublicationStatisticReport && filterType.HasValue)
                {
                    if (publicationStatsHandler is null)
                    {
                        return CustomResults.Problem(
                            title: "خطا در تولید گزارش",
                            detail: "Handler Ø¨Ø±Ø§ÛŒ Ø¢Ù…Ø§Ø± Ù†Ø´Ø±ÛŒØ§Øª ÛŒØ§ÙØª Ù†Ø´Ø¯",
                            statusCode: StatusCodes.Status500InternalServerError);
                    }

                    var query = new GetPublicationStatisticsQuery { FilterType = filterType.Value };
                    var result = await publicationStatsHandler.Handle(query, cancellationToken);

                    if (result.IsFailure)
                    {
                        return CustomResults.Problem(
                            title: "Ø®Ø·Ø§ Ø¯Ø± Ø¯Ø±ÛŒØ§ÙØª Ø¯Ø§Ø¯Ù‡\u200CÙ‡Ø§",
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
                            title: "ÙØ±Ù…Øª Ù†Ø§Ù…Ø¹ØªØ¨Ø±",
                            detail: $"ÙØ±Ù…Øª {format} Ù¾Ø´ØªÛŒØ¨Ø§Ù†ÛŒ Ù†Ù…ÛŒ\u200CØ´ÙˆØ¯. ÙØ±Ù…Øª\u200CÙ‡Ø§ÛŒ Ù…Ø¬Ø§Ø²: pdf, excel",
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
                    title: "Ú¯Ø²Ø§Ø±Ø´ ÛŒØ§ÙØª Ù†Ø´Ø¯",
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

