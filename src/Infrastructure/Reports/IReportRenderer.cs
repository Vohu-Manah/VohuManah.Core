namespace Infrastructure.Reports;

internal interface IReportRenderer
{
    byte[] RenderPdf(ReportModel reportModel);
    byte[] RenderExcel(ReportModel reportModel);
    string RenderHtml(ReportModel reportModel);
}

