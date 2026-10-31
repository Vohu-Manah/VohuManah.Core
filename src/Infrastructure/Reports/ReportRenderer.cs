using Infrastructure.Reports.Definition;
using Infrastructure.Reports.Html;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection;

namespace Infrastructure.Reports;

internal sealed class ReportRenderer : IReportRenderer
{
    private readonly ILogger<ReportRenderer> _logger;
    private readonly IReportHtmlWrapper _htmlWrapper;
    private readonly IReportTableGenerator _tableGenerator;
    private readonly RdlcDefinitionParser _definitionParser = new();
    private readonly RdlcHtmlTableRenderer _rdlcRenderer = new();
    private bool _fontsRegistered;
    private readonly object _fontLock = new();

    public ReportRenderer(
        ILogger<ReportRenderer> logger,
        IReportHtmlWrapper htmlWrapper,
        IReportTableGenerator tableGenerator)
    {
        _logger = logger;
        _htmlWrapper = htmlWrapper;
        _tableGenerator = tableGenerator;
    }

    public byte[] RenderPdf(ReportModel reportModel)
    {
        try
        {
            QuestPDF.Settings.License = LicenseType.Community;
            QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;
            EnsureShabnamFontsRegistered();

            var dataList = reportModel.DataSource.Cast<object>().ToList();
            var columns = ResolveColumns(reportModel, dataList);
            var definition = _definitionParser.Parse(reportModel.ReportPath);

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    
                    if (definition.IsLandscape)
                        page.Size(PageSizes.A4.Landscape());
                    else
                        page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Shabnam"));

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("صفحه ").FontSize(8).FontFamily("Shabnam");
                            text.CurrentPageNumber().FontSize(8).FontFamily("Shabnam");
                            text.Span(" از ").FontSize(8).FontFamily("Shabnam");
                            text.TotalPages().FontSize(8).FontFamily("Shabnam");
                        });

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            var reportTitle = "گزارش";
                            if (reportModel.Parameters.TryGetValue("Title", out object? titleObj) && titleObj != null)
                            {
                                reportTitle = titleObj.ToString() ?? "گزارش";
                            }

                            column.Item().Text(reportTitle)
                                .FontSize(18)
                                .Bold()
                                .FontFamily("Shabnam-Bold")
                                .AlignRight();

                            column.Item().PaddingTop(10, Unit.Millimetre);

                            if (dataList.Count > 0 && columns.Count > 0)
                            {
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(def =>
                                    {
                                        for (int i = 0; i < columns.Count; i++)
                                        {
                                            def.RelativeColumn();
                                        }
                                    });

                                    table.Header(header =>
                                    {
                                        foreach (var col in columns)
                                        {
                                            header.Cell().Element(cell => cell
                                                .Background(Colors.Grey.Lighten3)
                                                .Padding(5)
                                                .Text(col.Header)
                                                .FontSize(10)
                                                .FontFamily("Shabnam-Bold")
                                                .Bold()
                                                .AlignRight());
                                        }
                                    });

                                    var evaluator = new RdlcExpressionEvaluator(reportModel.Parameters);
                                    evaluator.ResetRows();
                                    foreach (object item in dataList)
                                    {
                                        foreach (var col in columns)
                                        {
                                            string value = evaluator.Evaluate(col.Expression, item);
                                            table.Cell().Element(cell => cell
                                                .BorderBottom(1)
                                                .BorderColor(Colors.Grey.Lighten2)
                                                .Padding(5)
                                                .Text(value)
                                                .FontSize(10)
                                                .FontFamily("Shabnam")
                                                .AlignRight());
                                        }
                                    }
                                });
                            }
                            else
                            {
                                column.Item().Text("داده برای نمایش وجود ندارد")
                                    .FontSize(12)
                                    .FontFamily("Shabnam")
                                    .AlignCenter();
                            }
                        });
                });
            })
            .GeneratePdf();

            return pdfBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در پردازش گزارش pdf: {ReportPath}", reportModel.ReportPath);
            throw new InvalidOperationException($"خطا در پردازش گزارش pdf '{reportModel.ReportPath}'. متن را بررسی کنید.", ex);
        }
    }

    private void EnsureShabnamFontsRegistered()
    {
        if (_fontsRegistered)
        {
            return;
        }

        lock (_fontLock)
        {
            if (_fontsRegistered)
            {
                return;
            }

            Assembly assembly = Assembly.GetExecutingAssembly();

            RegisterFont(assembly, "VohuManah.Infrastructure.Reports.Html.Resources.fonts.Shabnam-FD-WOL.ttf", "Shabnam");
            RegisterFont(assembly, "VohuManah.Infrastructure.Reports.Html.Resources.fonts.Shabnam-Bold-FD-WOL.ttf", "Shabnam-Bold");

            _fontsRegistered = true;
        }
    }

    private void RegisterFont(Assembly assembly, string resourceName, string fontName)
    {
        using Stream? fontStream = assembly.GetManifestResourceStream(resourceName);
        if (fontStream == null)
        {
            _logger.LogWarning("منبع فونت '{FontResource}' یافت نشد. برگشت به فونت های پیشفرض.", resourceName);
            return;
        }

        using var memoryStream = new MemoryStream();
        fontStream.CopyTo(memoryStream);
        memoryStream.Position = 0;
        FontManager.RegisterFontWithCustomName(fontName, memoryStream);
    }

    public byte[] RenderExcel(ReportModel reportModel)
    {
        try
        {
            ExcelPackage.License.SetNonCommercialOrganization(Assembly.GetCallingAssembly().GetName().Name);
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("گزارش");

            List<object> dataList = reportModel.DataSource.Cast<object>().ToList();
            var columns = ResolveColumns(reportModel, dataList);
            var definition = _definitionParser.Parse(reportModel.ReportPath);

            if (dataList.Count == 0 || columns.Count == 0)
            {
                worksheet.Cells[1, 1].Value = "داده ای برای نمایش وجود ندارد";
                worksheet.Cells[1, 1].Style.Font.Bold = true;
            }
            else
            {
                int colIndex = 1;
                foreach (var col in columns)
                {
                    worksheet.Cells[1, colIndex].Value = col.Header;
                    worksheet.Cells[1, colIndex].Style.Font.Bold = true;
                    worksheet.Cells[1, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[1, colIndex].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    colIndex++;
                }

                var evaluator = new RdlcExpressionEvaluator(reportModel.Parameters);
                evaluator.ResetRows();
                int rowIndex = 2;
                foreach (object item in dataList)
                {
                    colIndex = 1;
                    foreach (var col in columns)
                    {
                        worksheet.Cells[rowIndex, colIndex].Value = evaluator.Evaluate(col.Expression, item);
                        colIndex++;
                    }
                    rowIndex++;
                }

                worksheet.Cells.AutoFitColumns();
                worksheet.PrinterSettings.Orientation = definition.IsLandscape
                    ? eOrientation.Landscape
                    : eOrientation.Portrait;
            }

            return package.GetAsByteArray();
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
            var reportName = Path.GetFileNameWithoutExtension(reportModel.ReportPath);

            
            
            var reportHtml = RenderFromDefinition(reportModel);
            var definition = _definitionParser.Parse(reportModel.ReportPath);
            var orientation = definition.IsLandscape ? "landscape" : "portrait";
            return _htmlWrapper.Wrap(reportHtml, reportName, reportModel.Parameters, orientation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در پردازش گزارش html: {ReportPath}", reportModel.ReportPath);
            throw new InvalidOperationException($"خطا در پردازش گزارش html'{reportModel.ReportPath}'. متن را بررسی کنید.", ex);
        }
    }

    private string RenderFromDefinition(ReportModel reportModel)
    {
        try
        {
            var definition = _definitionParser.Parse(reportModel.ReportPath);
            if (definition.Columns.Count == 0 && !definition.IsChart)
            {
                return _tableGenerator.GenerateTable(reportModel);
            }

            return _rdlcRenderer.RenderHtml(definition, reportModel);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "تعریف RDLC خوانده نشد؛ استفاده از مسیر قدیمی: {ReportPath}", reportModel.ReportPath);
            return _tableGenerator.GenerateTable(reportModel);
        }
    }

    
    
    
    
    private List<ReportColumn> ResolveColumns(ReportModel reportModel, List<object> dataList)
    {
        try
        {
            var columns = _definitionParser.Parse(reportModel.ReportPath).Columns;
            if (columns.Count > 0)
                return columns.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "RDLC columns unreadable; falling back to properties: {ReportPath}", reportModel.ReportPath);
        }

        return dataList.Count == 0
            ? []
            : dataList[0].GetType().GetProperties()
                .Select(p => new ReportColumn { Header = p.Name, Expression = "=Fields!" + p.Name + ".Value" })
                .ToList();
    }
}

