using Microsoft.Extensions.Logging;
using System.Collections;

namespace Infrastructure.Reports;

internal sealed class RdlcReportService : IReportService
{
    private readonly ILogger<RdlcReportService> _logger;
    private readonly IReportLocalReportFactory _reportFactory;
    private readonly IReportRenderer _reportRenderer;

    public RdlcReportService(
        ILogger<RdlcReportService> logger,
        IReportLocalReportFactory reportFactory,
        IReportRenderer reportRenderer)
    {
        _logger = logger;
        _reportFactory = reportFactory;
        _reportRenderer = reportRenderer;
    }

    public async Task<byte[]> GeneratePdfAsync(
        string reportFileName,
        string reportDatasourceName,
        IEnumerable dataSource,
        Dictionary<string, object> parameters,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var reportModel = _reportFactory.Create(
                reportFileName,
                reportDatasourceName,
                dataSource,
                parameters);

            return await Task.FromResult(_reportRenderer.RenderPdf(reportModel));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ایحاد فایل pdf: {ReportFileName}", reportFileName);
            throw new InvalidOperationException($"خطا در ایحاد فایل pdf '{reportFileName}'. جزئیات برای بررسی بیشتر.", ex);
        }
    }

    public async Task<byte[]> GenerateExcelAsync(
        string reportFileName,
        string reportDatasourceName,
        IEnumerable dataSource,
        Dictionary<string, object> parameters,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var reportModel = _reportFactory.Create(
                reportFileName,
                reportDatasourceName,
                dataSource,
                parameters);

            return await Task.FromResult(_reportRenderer.RenderExcel(reportModel));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ایحاد فایل excel: {ReportFileName}", reportFileName);
            throw new InvalidOperationException($"خطا در ایحاد فایل excel '{reportFileName}'. جزئیات برای بررسی بیشتر.", ex);
        }
    }

    public async Task<string> GenerateHtmlAsync(
        string reportFileName,
        string reportDatasourceName,
        IEnumerable dataSource,
        Dictionary<string, object> parameters,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var reportModel = _reportFactory.Create(
                reportFileName,
                reportDatasourceName,
                dataSource,
                parameters);

            return await Task.FromResult(_reportRenderer.RenderHtml(reportModel));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ایحاد فایل html: {ReportFileName}", reportFileName);
            throw new InvalidOperationException($"خطا در ایحاد فایل html '{reportFileName}'. جزئیات برای بررسی بیشتر", ex);
        }
    }
}
