using System;
using System.Collections;
using System.Globalization;
using System.Net;
using System.Text;

namespace Infrastructure.Reports.Definition;





internal sealed class RdlcHtmlTableRenderer
{
    private const int DefaultChartHeight = 320;

    public string RenderHtml(ReportDefinition definition, ReportModel reportModel)
    {
        var evaluator = new RdlcExpressionEvaluator(reportModel.Parameters, pageNumber: 1, totalPages: 1);
        var sb = new StringBuilder();

        sb.Append("<div class='rdlc-report' dir='rtl'>");
        AppendHeader(sb, definition, evaluator);

        if (definition.IsChart)
        {
            AppendChart(sb, definition, reportModel, evaluator);
        }
        else
        {
            AppendTable(sb, definition, reportModel, evaluator);
        }

        AppendPageFooter(sb, definition, evaluator);

        sb.Append("</div>");
        return sb.ToString();
    }

    private static void AppendPageFooter(
        StringBuilder sb, ReportDefinition definition, RdlcExpressionEvaluator evaluator)
    {
        if (definition.PageFooterValues.Count == 0)
        {
            return;
        }

        sb.Append("<div class='rdlc-page-footer'>");
        foreach (string v in definition.PageFooterValues)
        {
            string rendered = evaluator.Evaluate(v, null);
            if (rendered.Contains('!'))
            {
                continue;
            }

            sb.Append("<span class='rdlc-page-footer-item'>")
              .Append(WebUtility.HtmlEncode(rendered.Length > 0 ? rendered : v))
              .Append("</span> ");
        }

        sb.Append("</div>");
    }

    private static void AppendHeader(
        StringBuilder sb, ReportDefinition definition, RdlcExpressionEvaluator evaluator)
    {
        sb.Append("<div class='rdlc-header'>");

        string title = evaluator.Evaluate(definition.TitleExpression, null);
        if (title.Length > 0)
        {
            sb.Append("<h2>").Append(WebUtility.HtmlEncode(title)).Append("</h2>");
        }

        if (definition.HeaderStats.Count > 0)
        {
            sb.Append("<div class='rdlc-header-meta'>");
            foreach (ReportStat stat in definition.HeaderStats)
            {
                sb.Append("<span class='rdlc-stat'>")
                  .Append(WebUtility.HtmlEncode(stat.Label))
                  .Append(' ')
                  .Append(WebUtility.HtmlEncode(evaluator.Evaluate(stat.Expression, null)))
                  .Append("</span>");
            }

            sb.Append("</div>");
        }

        sb.Append("</div>");

        
        if (definition.BodyStats.Count > 0 && !definition.Columns.Any())
        {
            sb.Append("<div class='rdlc-body-stats'>");
            foreach (ReportStat stat in definition.BodyStats)
            {
                sb.Append("<div class='rdlc-stat'>")
                  .Append(WebUtility.HtmlEncode(stat.Label))
                  .Append(' ')
                  .Append("<b>").Append(WebUtility.HtmlEncode(evaluator.Evaluate(stat.Expression, null))).Append("</b>")
                  .Append("</div>");
            }

            sb.Append("</div>");
        }
    }

    private static void AppendTable(
        StringBuilder sb, ReportDefinition definition, ReportModel reportModel, RdlcExpressionEvaluator evaluator)
    {
        List<object?> items = reportModel.DataSource.Cast<object?>().ToList();
        if (items.Count == 0)
        {
            sb.Append("<p class='rdlc-empty'>داده\u200Cای برای نمایش وجود ندارد</p>");
            return;
        }

        sb.Append("<table class='rdlc-table'><thead><tr>");
        foreach (ReportColumn column in definition.Columns)
        {
            sb.Append("<th>").Append(WebUtility.HtmlEncode(column.Header)).Append("</th>");
        }

        sb.Append("</tr></thead><tbody>");

        evaluator.ResetRows();
        foreach (object? item in items)
        {
            sb.Append("<tr>");
            foreach (ReportColumn column in definition.Columns)
            {
                string value = evaluator.Evaluate(column.Expression, item);
                sb.Append("<td>").Append(WebUtility.HtmlEncode(value)).Append("</td>");
            }

            sb.Append("</tr>");
        }

        sb.Append("</tbody></table>");
    }

    
    
    
    
    private static void AppendChart(
        StringBuilder sb, ReportDefinition definition, ReportModel reportModel, RdlcExpressionEvaluator evaluator)
    {
        ReportChart chart = definition.Chart!;

        var points = new List<(string Label, double Value)>();
        foreach (object? item in reportModel.DataSource.Cast<object?>())
        {
            string label = evaluator.Evaluate(chart.CategoryExpression, item);
            string raw = evaluator.Evaluate(chart.ValueExpression, item);
            double value = double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsed)
                ? parsed
                : 0;

            points.Add((label, value));
        }

        if (points.Count == 0)
        {
            sb.Append("<p class='rdlc-empty'>داده\u200Cای برای نمایش وجود ندارد</p>");
            return;
        }

        const int barWidth = 52;
        const int gap = 18;
        const int leftPad = 54;
        const int topPad = 16;

        double max = points.Max(p => p.Value);
        if (max <= 0)
        {
            max = 1;
        }

        int width = leftPad + (points.Count * (barWidth + gap)) + gap;
        int height = DefaultChartHeight;
        int plotHeight = height - topPad - 46;

        sb.Append("<div class='rdlc-chart'>");
        sb.Append("<svg viewBox='0 0 ").Append(width).Append(' ').Append(height)
          .Append("' width='100%' height='").Append(height).Append("' preserveAspectRatio='xMidYMid meet'>");

        
        sb.Append("<line x1='").Append(leftPad).Append("' y1='").Append(topPad + plotHeight)
          .Append("' x2='").Append(width - gap).Append("' y2='").Append(topPad + plotHeight)
          .Append("' stroke='#adb5bd' stroke-width='1'/>");

        
        for (int i = 1; i <= 4; i++)
        {
            double ratio = i / 4.0;
            int y = topPad + plotHeight - (int)(plotHeight * ratio);
            sb.Append("<line x1='").Append(leftPad).Append("' y1='").Append(y)
              .Append("' x2='").Append(width - gap).Append("' y2='").Append(y)
              .Append("' stroke='#e9ecef' stroke-width='1'/>")
              .Append("<text x='").Append(leftPad - 6).Append("' y='").Append(y + 4)
              .Append("' text-anchor='end' font-size='11' fill='#868e96'>")
              .Append(WebUtility.HtmlEncode(Math.Round(max * ratio).ToString(CultureInfo.InvariantCulture)))
              .Append("</text>");
        }

        for (int i = 0; i < points.Count; i++)
        {
            (string label, double value) = points[i];

            int barHeight = (int)(plotHeight * (value / max));
            int x = leftPad + gap + (i * (barWidth + gap));
            int y = topPad + plotHeight - barHeight;

            sb.Append("<rect x='").Append(x).Append("' y='").Append(y)
              .Append("' width='").Append(barWidth).Append("' height='").Append(barHeight)
              .Append("' fill='#228be6' rx='3'/>");

            if (chart.ShowDataLabels)
            {
                sb.Append("<text x='").Append(x + (barWidth / 2)).Append("' y='").Append(y - 6)
                  .Append("' text-anchor='middle' font-size='12' font-weight='bold' fill='#212529'>")
                  .Append(WebUtility.HtmlEncode(value.ToString(CultureInfo.InvariantCulture)))
                  .Append("</text>");
            }

            
            sb.Append("<text x='").Append(x + (barWidth / 2)).Append("' y='").Append(topPad + plotHeight + 18)
              .Append("' text-anchor='middle' font-size='11' fill='#495057'>")
              .Append(WebUtility.HtmlEncode(Truncate(label, 14)))
              .Append("</text>");
        }

        sb.Append("</svg></div>");
    }

    private static string Truncate(string value, int max)
    {
        if (value.Length <= max)
        {
            return value;
        }

        return value[..(max - 1)] + "…";
    }
}