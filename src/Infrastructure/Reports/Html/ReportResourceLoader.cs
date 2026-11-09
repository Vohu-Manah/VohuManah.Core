using System.IO;
using System;
using System.Reflection;
using System.Text;

namespace Infrastructure.Reports.Html;

internal sealed class ReportResourceLoader : IReportResourceLoader
{
    private static readonly Assembly Assembly = typeof(ReportResourceLoader).Assembly;
    private const string ResourceBasePath = "VohuManah.Infrastructure.Reports.Html.Resources";

    public string LoadStyles()
    {
        return LoadResource("ReportStyles.css");
    }

    public string LoadScripts()
    {
        return LoadResource("ReportScripts.js");
    }

    public byte[] LoadFont(string resourceName)
    {
        return LoadResourceBytes(resourceName);
    }

    public string LoadFontAwesomeCss()
    {
        return LoadResource("fontawesome.all.min.css");
    }

    public byte[] LoadFontAwesomeFont(string fontName)
    {
        return LoadResourceBytes($"fontawesome.{fontName}");
    }

    private static string LoadResource(string resourceName)
    {
        var fullResourceName = $"{ResourceBasePath}.{resourceName}";

        using var stream = Assembly.GetManifestResourceStream(fullResourceName);
        if (stream == null)
        {
            throw new InvalidOperationException($"منابع '{fullResourceName}'یافت نشد.منابع موجود: {string.Join(", ", Assembly.GetManifestResourceNames())}");
        }

        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    private static byte[] LoadResourceBytes(string resourceName)
    {
        var fullResourceName = $"{ResourceBasePath}.{resourceName}";

        using var stream = Assembly.GetManifestResourceStream(fullResourceName);
        if (stream == null)
        {
            throw new InvalidOperationException($"منابع '{fullResourceName}' یافت نشد.منابع موجود: {string.Join(", ", Assembly.GetManifestResourceNames())}");
        }

        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}

