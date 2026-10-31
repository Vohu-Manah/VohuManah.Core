namespace Infrastructure.Reports.Html;

internal interface IReportTableGenerator
{
    string GenerateTable(ReportModel reportModel);
}

