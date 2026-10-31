namespace Infrastructure.Reports.Html;

internal interface IHtmlBuilder
{
    HtmlBuilder Append(string text);
    HtmlBuilder AppendLine(string? text = null);
    HtmlBuilder AppendIndented(string text);
    HtmlElement OpenTag(string tagName, Dictionary<string, string>? attributes = null);
    HtmlBuilder CloseTag(string tagName);
    HtmlBuilder SelfClosingTag(string tagName, Dictionary<string, string>? attributes = null);
    HtmlBuilder Text(string text);
    HtmlBuilder Raw(string html);
    string ToString();
}

