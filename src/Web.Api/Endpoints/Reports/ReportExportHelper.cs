using Infrastructure.Reports;
using System.Collections;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Reports;

internal static class ReportExportHelper
{
    public static async Task<IResult> ExportReportAsync(
        IEnumerable dataSource,
        string reportName,
        string datasourceName,
        string format,
        IReportService reportService,
        Dictionary<string, object>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        if (dataSource is ICollection collection && collection.Count == 0)
        {
            return CustomResults.Problem(
                title: "گزارش خالی",
                detail: "گزارش درخواستی خالی می باشد",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var reportParameters = parameters ?? new Dictionary<string, object>();
        reportParameters["Today"] = DateTime.Now.ToString("yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture);

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
                    reportParameters,
                    cancellationToken);
                contentType = "application/pdf";
                fileName = $"{GetReportFileName(reportName)}.pdf";
                break;

            case "EXCEL":
            case "XLSX":
                fileBytes = await reportService.GenerateExcelAsync(
                    reportName,
                    datasourceName,
                    dataSource,
                    reportParameters,
                    cancellationToken);
                contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                fileName = $"{GetReportFileName(reportName)}.xlsx";
                break;

            default:
                return CustomResults.Problem(
                    title: "فرمت نامعتبر",
                    detail: $"فرمت {format} پشتیبانی نمی شود. فرمت های مجاز: pdf, excel",
                    statusCode: StatusCodes.Status400BadRequest);
        }

        return Results.File(fileBytes, contentType, fileName);
    }

    public static IResult HandleExportException(Exception ex, string format)
    {
        return ex switch
        {
            NotSupportedException => CustomResults.Problem(
                title: "PDF Export در دسترس نیست",
                detail: ex.Message,
                statusCode: StatusCodes.Status501NotImplemented),
            FileNotFoundException => CustomResults.Problem(
                title: "گزارش یافت نشد",
                detail: ex.Message,
                statusCode: StatusCodes.Status404NotFound),
            _ => CustomResults.Problem(
                title: "خطا در تولید گزارش",
                detail: $"Failed to generate {format.ToUpperInvariant()} report. {ex.Message}",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }

    private static string GetReportFileName(string reportName)
    {
        return reportName switch
        {
            ReportConstants.BookSearchListReport => "BookSearchList",
            ReportConstants.ManuscriptSearchListReport => "ManuscriptSearchList",
            ReportConstants.PublicationSearchListReport => "PublicationSearchList",
            _ => reportName
        };
    }
}

