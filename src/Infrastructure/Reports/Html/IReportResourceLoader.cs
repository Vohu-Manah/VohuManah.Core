namespace Infrastructure.Reports.Html;

internal interface IReportResourceLoader
{
    string LoadStyles();
    string LoadScripts();
    byte[] LoadFont(string resourceName);
    string LoadFontAwesomeCss();
    byte[] LoadFontAwesomeFont(string fontName);
}

