using System.Xml.Linq;

namespace Infrastructure.Reports.Definition;

internal sealed class RdlcDefinitionParser
{
    public ReportDefinition Parse(string reportPath)
    {
        XDocument doc = XDocument.Load(reportPath);
        XElement root = doc.Root ?? throw new InvalidDataException("فایل گزارش خالی است");

        XElement? body = All(root, "Body").FirstOrDefault();
        XElement? header = All(root, "PageHeader").FirstOrDefault();
        XElement? footer = All(root, "PageFooter").FirstOrDefault();
        XElement? titleTextbox = ParseTitleTextbox(header);
        string title = titleTextbox is null ? string.Empty : TextboxValue(titleTextbox);

        return new ReportDefinition
        {
            TitleExpression = title,
            HeaderStats = ParseStats(header, titleTextbox),
            BodyStats = ParseStats(body),
            Columns = ParseTablixColumns(root),
            Chart = ParseChart(root),
            PageWidth = ParsePageSize(root, "PageWidth", "11.69in"),
            PageHeight = ParsePageSize(root, "PageHeight", "8.27in"),
            PageHeaderValues = ParseSectionTexts(header, titleTextbox),
            PageFooterValues = ParseSectionTexts(footer, null),
        };
    }

    private static IEnumerable<XElement> All(XElement root, string localName) =>
        root.Descendants().Where(e => e.Name.LocalName == localName);

    private static XElement? ParseTitleTextbox(XElement? header)
    {
        if (header is null)
        {
            return null;
        }

        foreach (XElement textbox in All(header, "Textbox"))
        {
            string value = TextboxValue(textbox);
            if (value.Length > 0)
            {
                return textbox;
            }
        }

        return null;
    }

    private static string TextboxValue(XElement textbox) =>
        All(textbox, "Value").Select(v => v.Value.Trim())
            .FirstOrDefault(v => v.Length > 0) ?? string.Empty;

    private static List<string> ParseSectionTexts(XElement? section, XElement? excludeTitleTextbox = null)
    {
        var values = new List<string>();
        if (section is null) return values;
        string excludeValue = excludeTitleTextbox is null ? string.Empty : TextboxValue(excludeTitleTextbox);
        foreach (XElement textbox in All(section, "Textbox"))
        {
            if (textbox == excludeTitleTextbox)
            {
                continue;
            }

            string value = TextboxValue(textbox);
            if (value.Length == 0)
            {
                continue;
            }

            if (excludeValue.Length > 0 &&
                string.Equals(value.TrimEnd(':', ' '), excludeValue.TrimEnd(':', ' '), StringComparison.Ordinal))
            {
                continue;
            }

            values.Add(value);
        }
        return values;
    }

    private static string ParsePageSize(XElement root, string localName, string defaultValue)
    {
        var value = root.Descendants()
            .Where(e => e.Name.LocalName == localName)
            .Select(e => e.Value.Trim())
            .FirstOrDefault(v => v.Length > 0);

        return string.IsNullOrWhiteSpace(value) ? defaultValue : value!;
    }

    private static List<ReportStat> ParseStats(XElement? container, XElement? excludeTitleTextbox = null)
    {
        var stats = new List<ReportStat>();
        if (container is null)
        {
            return stats;
        }

        var labels = new List<string>();
        var values = new List<string>();

        string excludeValue = excludeTitleTextbox is null ? string.Empty : TextboxValue(excludeTitleTextbox);

        foreach (XElement textbox in All(container, "Textbox"))
        {
            if (textbox.Ancestors().Any(a => a.Name.LocalName == "Tablix"))
            {
                continue;
            }

            string value = All(textbox, "Value").Select(v => v.Value.Trim())
                .FirstOrDefault(v => v.Length > 0) ?? string.Empty;

            if (value.Length == 0)
            {
                continue;
            }

            if (value.StartsWith('='))
            {
                if (excludeValue.Length > 0 &&
                    string.Equals(value, excludeValue, StringComparison.Ordinal))
                {
                    continue;
                }

                values.Add(value);
            }
            else
            {
                string label = value.TrimEnd(':', ' ', '\u200c');
                if (excludeValue.Length > 0 &&
                    string.Equals(label, excludeValue.TrimEnd(':', ' ', '\u200c'), StringComparison.Ordinal))
                {
                    continue;
                }

                labels.Add(label);
            }
        }

        int count = Math.Max(labels.Count, values.Count);
        for (int i = 0; i < count; i++)
        {
            stats.Add(new ReportStat
            {
                Label = i < labels.Count ? labels[i] : string.Empty,
                Expression = i < values.Count ? values[i] : string.Empty,
            });
        }

        return stats;
    }

    private static List<ReportColumn> ParseTablixColumns(XElement root)
    {
        XElement? tablix = All(root, "Tablix").FirstOrDefault();
        if (tablix is null)
        {
            return new List<ReportColumn>();
        }

        List<XElement> rows = All(tablix, "TablixRow").ToList();
        if (rows.Count < 2)
        {
            return new List<ReportColumn>();
        }

        List<string> headers = CellValues(rows[0]);
        List<string> expressions = CellValues(rows[1]);

        var columns = new List<ReportColumn>();
        int count = Math.Min(headers.Count, expressions.Count);
        for (int i = 0; i < count; i++)
        {
            columns.Add(new ReportColumn { Header = headers[i], Expression = expressions[i] });
        }
        return columns;
    }

    private static List<string> CellValues(XElement row)
    {
        var result = new List<string>();
        foreach (XElement cell in All(row, "TablixCell"))
        {
            string value = All(cell, "Value").Select(v => v.Value.Trim())
                .FirstOrDefault(v => v.Length > 0) ?? string.Empty;
            result.Add(value);
        }
        return result;
    }

    private static ReportChart? ParseChart(XElement root)
    {
        XElement? chart = All(root, "Chart").FirstOrDefault();
        if (chart is null)
        {
            return null;
        }

        XElement? values = All(chart, "ChartDataPointValues").FirstOrDefault();
        if (values is null)
        {
            return null;
        }

        string category = All(values, "X").Select(v => v.Value.Trim()).FirstOrDefault() ?? string.Empty;
        string value = All(values, "Y").Select(v => v.Value.Trim()).FirstOrDefault() ?? string.Empty;
        if (category.Length == 0 || value.Length == 0)
        {
            return null;
        }

        return new ReportChart { CategoryExpression = category, ValueExpression = value };
    }
}

