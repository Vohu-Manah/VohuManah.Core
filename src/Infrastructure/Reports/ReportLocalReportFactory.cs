using Infrastructure.Helper;
using System.Collections;

namespace Infrastructure.Reports;

internal sealed class ReportLocalReportFactory : IReportLocalReportFactory
{
    private readonly string _reportsPath;
    private readonly ReportParameterBuilder _parameterBuilder;

    public ReportLocalReportFactory(string reportsPath, PersianDateTime persianDateTime)
    {
        _reportsPath = reportsPath;
        _parameterBuilder = new ReportParameterBuilder(persianDateTime);
    }

    public ReportModel Create(
        string reportFileName,
        string reportDatasourceName,
        IEnumerable dataSource,
        Dictionary<string, object> parameters)
    {
        var reportPath = GetReportPath(reportFileName);
        ValidateReportExists(reportPath, reportFileName);

        var reportModel = new ReportModel
        {
            ReportPath = reportPath,
            DataSourceName = reportDatasourceName,
            DataSource = dataSource,
            Parameters = BuildParameters(parameters)
        };

        return reportModel;
    }

    private string GetReportPath(string reportFileName)
    {
        return Path.Combine(_reportsPath, $"{reportFileName}.rdlc");
    }

    private static void ValidateReportExists(string reportPath, string reportFileName)
    {
        if (!File.Exists(reportPath))
        {
            throw new FileNotFoundException($"گزارش {reportFileName} یافت نشد", reportPath);
        }
    }

    private Dictionary<string, object> BuildParameters(Dictionary<string, object> parameters)
    {
        var builder = _parameterBuilder.AddToday();
        builder.AddRange(parameters);
        return builder.Build();
    }
}

