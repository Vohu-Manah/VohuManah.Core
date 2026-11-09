using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Infrastructure.Reports.Definition;






internal sealed class RdlcExpressionEvaluator
{
    private static readonly Regex FieldPattern = new(@"Fields!(\w+)\.Value", RegexOptions.Compiled);
    private static readonly Regex ParamPattern = new(@"Parameters!(\w+)\.Value", RegexOptions.Compiled);
    private static readonly Regex RowNumberPattern = new(@"RowNumber\(", RegexOptions.Compiled);
    private static readonly Regex ExecTimePattern = new(@"Globals!ExecutionTime", RegexOptions.Compiled);
    private static readonly Regex PageNumberPattern = new(@"Globals!PageNumber", RegexOptions.Compiled);
    private static readonly Regex TotalPagesPattern = new(@"Globals!OverallTotalPages", RegexOptions.Compiled);

    private readonly Dictionary<string, object> _parameters;
    private int _rowNumber;
    private readonly int _pageNumber;
    private readonly int _totalPages;

    public RdlcExpressionEvaluator(Dictionary<string, object> parameters)
        : this(parameters, pageNumber: 1, totalPages: 1)
    {
    }

    public RdlcExpressionEvaluator(Dictionary<string, object> parameters, int pageNumber, int totalPages)
    {
        _parameters = parameters;
        _pageNumber = pageNumber;
        _totalPages = totalPages;
    }

    public void ResetRows() => _rowNumber = 0;

    
    public string Evaluate(string expression, object? item)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return string.Empty;
        }

        string expr = expression.Trim();
        if (!expr.StartsWith('='))
        {
            return expr; 
        }

        expr = expr[1..];

        if (RowNumberPattern.IsMatch(expr))
        {
            _rowNumber++;
            return _rowNumber.ToString(CultureInfo.InvariantCulture);
        }

        if (ExecTimePattern.IsMatch(expr))
        {
            return DateTime.Now.ToString("HH:mm", CultureInfo.InvariantCulture);
        }

        Match pageNum = PageNumberPattern.Match(expr);
        if (pageNum.Success)
        {
            return _pageNumber.ToString(CultureInfo.InvariantCulture);
        }

        Match totalPages = TotalPagesPattern.Match(expr);
        if (totalPages.Success)
        {
            return _totalPages.ToString(CultureInfo.InvariantCulture);
        }

        Match param = ParamPattern.Match(expr);
        if (param.Success)
        {
            string name = param.Groups[1].Value;
            return _parameters.TryGetValue(name, out object? value)
                ? Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
                : string.Empty;
        }

        Match field = FieldPattern.Match(expr);
        if (field.Success)
        {
            return ReadProperty(item, field.Groups[1].Value);
        }

        
        return expr;
    }

    private static string ReadProperty(object? item, string propertyName)
    {
        if (item is null)
        {
            return string.Empty;
        }

        
        PropertyInfo? prop = item.GetType().GetProperty(
            propertyName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

        object? value = prop?.GetValue(item);

        if (value is null)
        {
            return string.Empty;
        }

        if (value is not string && value is IEnumerable)
        {
            return string.Empty;
        }

        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
    }
}