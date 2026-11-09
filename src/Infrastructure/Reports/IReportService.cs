using System.Collections;

namespace Infrastructure.Reports;

public interface IReportService
{
    Task<byte[]> GeneratePdfAsync(
        string reportFileName,
        string reportDatasourceName,
        IEnumerable dataSource,
        Dictionary<string, object> parameters,
        CancellationToken cancellationToken = default);

    Task<byte[]> GenerateExcelAsync(
        string reportFileName,
        string reportDatasourceName,
        IEnumerable dataSource,
        Dictionary<string, object> parameters,
        CancellationToken cancellationToken = default);

    Task<string> GenerateHtmlAsync(
        string reportFileName,
        string reportDatasourceName,
        IEnumerable dataSource,
        Dictionary<string, object> parameters,
        CancellationToken cancellationToken = default);
}

