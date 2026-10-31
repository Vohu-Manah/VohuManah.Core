namespace Infrastructure.Reports.Html;

internal interface IReportHtmlWrapper
{
    string Wrap(string reportHtml, string reportName, Dictionary<string, object> parameters, string orientation = "portrait");
}

