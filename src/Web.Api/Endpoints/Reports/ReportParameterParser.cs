namespace Web.Api.Endpoints.Reports;

internal static class ReportParameterParser
{
    public static Dictionary<string, object> Parse(string? parameters)
    {
        var result = new Dictionary<string, object>();

        if (string.IsNullOrWhiteSpace(parameters))
        {
            return result;
        }

        var pairs = parameters.Split('&', StringSplitOptions.RemoveEmptyEntries);
        foreach (var pair in pairs)
        {
            var keyValue = pair.Split('=', 2);
            if (keyValue.Length == 2)
            {
                var key = Uri.UnescapeDataString(keyValue[0]);
                var value = Uri.UnescapeDataString(keyValue[1]);
                result[key] = value;
            }
        }

        return result;
    }
}
