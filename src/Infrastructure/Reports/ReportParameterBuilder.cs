using Infrastructure.Helper;

namespace Infrastructure.Reports;

internal sealed class ReportParameterBuilder
{
    private readonly PersianDateTime _persianDateTime;
    private readonly Dictionary<string, object> _parameters;

    public ReportParameterBuilder(PersianDateTime persianDateTime)
    {
        _persianDateTime = persianDateTime;
        _parameters = new Dictionary<string, object>();
    }

    public ReportParameterBuilder AddToday()
    {
        _parameters["Today"] = _persianDateTime.PerianDate();
        return this;
    }

    public ReportParameterBuilder Add(string key, object? value)
    {
        if (value != null)
        {
            _parameters[key] = value;
        }
        return this;
    }

    public ReportParameterBuilder AddRange(Dictionary<string, object> parameters)
    {
        foreach (var param in parameters)
        {
            if (param.Value != null)
            {
                _parameters[param.Key] = param.Value;
            }
        }
        return this;
    }

    public Dictionary<string, object> Build()
    {
        return new Dictionary<string, object>(_parameters);
    }
}

