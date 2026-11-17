using System.Collections;

namespace VohuManah.Infrastructure.Reports;

internal interface IReportLocalReportFactory
{
    ReportModel Create(
        string reportFileName,
        string reportDatasourceName,
        IEnumerable dataSource,
        Dictionary<string, object> parameters);
}

