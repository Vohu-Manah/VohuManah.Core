using System;
using System.Globalization;
using System.Text;

namespace Infrastructure.Reports.Html;

internal sealed class HtmlBuilder : IHtmlBuilder
{
    private readonly StringBuilder _builder = new();
    private int _indentLevel;
    private const int IndentSize = 2;

    public HtmlBuilder Append(string text)
    {
        _builder.Append(text);
        return this;
    }

    public HtmlBuilder AppendLine(string? text = null)
    {
        if (text != null)
        {
            _builder.Append(new string(' ', _indentLevel * IndentSize));
            _builder.AppendLine(text);
        }
        else
        {
            _builder.AppendLine();
        }
        return this;
    }

    public HtmlBuilder AppendIndented(string text)
    {
        _builder.Append(new string(' ', _indentLevel * IndentSize));
        _builder.Append(text);
        return this;
    }

    public HtmlElement OpenTag(string tagName, Dictionary<string, string>? attributes = null)
    {
        string indent = new string(' ', _indentLevel * IndentSize);
        _builder.Append(indent);
        _builder.Append(CultureInfo.InvariantCulture, $"<{tagName}");

        if (attributes != null && attributes.Count > 0)
        {
            foreach (KeyValuePair<string, string> attr in attributes)
            {
                _builder.Append(CultureInfo.InvariantCulture, $" {attr.Key}=\"{attr.Value}\"");
            }
        }

        _builder.AppendLine(">");
        _indentLevel++;

        return new HtmlElement(this, tagName);
    }

    public HtmlBuilder CloseTag(string tagName)
    {
        _indentLevel--;
        AppendIndented($"</{tagName}>");
        AppendLine();
        return this;
    }

    public HtmlBuilder SelfClosingTag(string tagName, Dictionary<string, string>? attributes = null)
    {
        string indent = new string(' ', _indentLevel * IndentSize);
        _builder.Append(indent);
        _builder.Append(CultureInfo.InvariantCulture, $"<{tagName}");

        if (attributes != null && attributes.Count > 0)
        {
            foreach (KeyValuePair<string, string> attr in attributes)
            {
                _builder.Append(CultureInfo.InvariantCulture, $" {attr.Key}=\"{attr.Value}\"");
            }
        }

        _builder.AppendLine(" />");
        return this;
    }

    public HtmlBuilder Text(string text)
    {
        AppendIndented(text);
        AppendLine();
        return this;
    }

    public HtmlBuilder Raw(string html)
    {
        _builder.Append(html);
        return this;
    }

    public override string ToString()
    {
        return _builder.ToString();
    }
}

internal sealed class HtmlElement : IDisposable
{
    private readonly HtmlBuilder _builder;
    private readonly string _tagName;
    private bool _disposed;

    public HtmlElement(HtmlBuilder builder, string tagName)
    {
        _builder = builder;
        _tagName = tagName;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _builder.CloseTag(_tagName);
            _disposed = true;
        }
    }
}

