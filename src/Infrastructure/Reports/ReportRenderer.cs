using Infrastructure.Reports.Html;
using Microsoft.Extensions.Logging;
using Reporting.NETCore;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace Infrastructure.Reports;

internal sealed class ReportRenderer : IReportRenderer
{
    private readonly ILogger<ReportRenderer> _logger;
    private readonly IReportHtmlWrapper _htmlWrapper;

    public ReportRenderer(
        ILogger<ReportRenderer> logger,
        IReportHtmlWrapper htmlWrapper)
    {
        _logger = logger;
        _htmlWrapper = htmlWrapper;
    }

    public byte[] RenderPdf(ReportModel reportModel)
    {
        try
        {
            using var report = CreateReport(reportModel);
            byte[] pdf = report.Render(
                "PDF",
                deviceInfo: "<DeviceInfo><OutputFormat>PDF</OutputFormat><EmbedFonts>Full</EmbedFonts></DeviceInfo>",
                pageCountMode: PageCountMode.Actual,
                mimeType: out _,
                encoding: out _,
                fileNameExtension: out _,
                streams: out _,
                warnings: out Warning[] warnings);

            LogWarnings(warnings);
            return pdf;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در پردازش گزارش pdf: {ReportPath}", reportModel.ReportPath);
            throw new InvalidOperationException($"خطا در پردازش گزارش pdf '{reportModel.ReportPath}'. متن را بررسی کنید.", ex);
        }
    }

    public byte[] RenderExcel(ReportModel reportModel)
    {
        try
        {
            using var report = CreateReport(reportModel);
            byte[] excel = report.Render(
                "EXCELOPENXML",
                deviceInfo: null,
                pageCountMode: PageCountMode.Actual,
                mimeType: out _,
                encoding: out _,
                fileNameExtension: out _,
                streams: out _,
                warnings: out Warning[] warnings);

            LogWarnings(warnings);
            return excel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در پردازش گزارش excel: {ReportPath}", reportModel.ReportPath);
            throw new InvalidOperationException($"خطا در پردازش گزارش excel '{reportModel.ReportPath}'. متن را بررسی کنید.", ex);
        }
    }

    public string RenderHtml(ReportModel reportModel)
    {
        try
        {
            using var report = CreateReport(reportModel);
            
            
            
            
            
            
            const string deviceInfo =
                "<DeviceInfo>" +
                "<OutputFormat>PDF</OutputFormat>" + 
                "<PageHeight>29.7cm</PageHeight>" +
                "<PageWidth>21cm</PageWidth>" +
                "<MarginTop>1cm</MarginTop>" +
                "<MarginLeft>1cm</MarginLeft>" +
                "<MarginRight>1cm</MarginRight>" +
                "<MarginBottom>1cm</MarginBottom>" +
                "</DeviceInfo>";
            byte[] htmlBytes = report.Render(
                "HTML4.0",
                deviceInfo: deviceInfo,
                pageCountMode: PageCountMode.Actual,
                mimeType: out _,
                encoding: out _,
                fileNameExtension: out _,
                streams: out _,
                warnings: out Warning[] warnings);

            LogWarnings(warnings);
            string reportHtml = Encoding.UTF8.GetString(htmlBytes);
            var reportName = Path.GetFileNameWithoutExtension(reportModel.ReportPath);
            return _htmlWrapper.Wrap(reportHtml, reportName, reportModel.Parameters, "portrait");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در پردازش گزارش html: {ReportPath}", reportModel.ReportPath);
            throw new InvalidOperationException($"خطا در پردازش گزارش html '{reportModel.ReportPath}'. متن را بررسی کنید.", ex);
        }
    }

    private LocalReport CreateReport(ReportModel reportModel)
    {
        var report = new LocalReport
        {
            ReportPath = reportModel.ReportPath
        };

        var dataSource = new ReportDataSource(reportModel.DataSourceName, reportModel.DataSource);
        report.DataSources.Add(dataSource);

        
        
        
        var defined = new HashSet<string>(
            report.GetParameters().Select(p => p.Name),
            StringComparer.OrdinalIgnoreCase);
        var parameters = reportModel.Parameters
            .Where(kv => defined.Contains(kv.Key))
            .Select(kv => new ReportParameter(kv.Key, kv.Value?.ToString() ?? string.Empty))
            .ToArray();
        report.SetParameters(parameters);

        report.Refresh();
        return report;
    }

    private static void LogWarnings(Warning[] warnings)
    {
        if (warnings == null)
            return;

        foreach (var warning in warnings)
        {
            
        }
    }
}
