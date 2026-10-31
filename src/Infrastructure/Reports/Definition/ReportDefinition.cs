namespace Infrastructure.Reports.Definition;





internal sealed class ReportDefinition
{
    
    public string TitleExpression { get; init; } = string.Empty;

    
    public IReadOnlyList<ReportStat> HeaderStats { get; init; } = [];

    
    public IReadOnlyList<ReportStat> BodyStats { get; init; } = [];

    public IReadOnlyList<ReportColumn> Columns { get; init; } = [];

    
    public IReadOnlyList<string> PageHeaderValues { get; init; } = [];

    
    public IReadOnlyList<string> PageFooterValues { get; init; } = [];

    
    public ReportChart? Chart { get; init; }

    public bool IsChart => Chart is not null;
    public string PageWidth { get; init; } = "8.27in";
    public string PageHeight { get; init; } = "11.69in";

    public bool IsLandscape => ParseSize(PageWidth) > ParseSize(PageHeight);

    private static double ParseSize(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0;
        value = value.Trim();
        int i = 0;
        while (i < value.Length && (char.IsDigit(value[i]) || value[i] == '.' || value[i] == ',')) i++;
        var numStr = value.Substring(0, i).Replace(',', '.');
        if (!double.TryParse(numStr, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out var n)) return 0;
        var unit = value.Substring(i).Trim().ToUpperInvariant();
        return unit switch
        {
            "CM" => n / 2.54,
            "MM" => n / 25.4,
            "PX" => n / 96.0,
            _ => n,
        };
    }
}


internal sealed class ReportStat
{
    public required string Label { get; init; }

    public required string Expression { get; init; }
}

internal sealed class ReportColumn
{
    public required string Header { get; init; }

    
    public required string Expression { get; init; }
}

internal sealed class ReportChart
{
    public required string CategoryExpression { get; init; }

    public required string ValueExpression { get; init; }

    public bool ShowDataLabels { get; init; }
}
