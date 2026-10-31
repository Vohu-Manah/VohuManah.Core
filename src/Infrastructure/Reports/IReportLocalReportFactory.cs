using System.Collections;

namespace Infrastructure.Reports;

internal interface IReportLocalReportFactory
{
    ReportModel Create(
        string reportFileName,
        string reportDatasourceName,
        IEnumerable dataSource,
        Dictionary<string, object> parameters);
}

