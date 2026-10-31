using System.Collections;

namespace Infrastructure.Reports;

internal sealed class ReportModel
{
    public required string ReportPath { get; init; }
    public required string DataSourceName { get; init; }
    public required IEnumerable DataSource { get; init; }
    public required Dictionary<string, object> Parameters { get; init; }
}

