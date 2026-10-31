using System.Reflection;

namespace Web.Api.Endpoints.Reports;

internal sealed class FontResource : BaseEndpoint
{
    public override void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("reports/fonts/{fontName}", (string fontName) =>
        {
            try
            {
                Assembly assembly = Assembly.Load("VohuManah.Infrastructure");
                string resourceName = $"VohuManah.Infrastructure.Reports.Html.Resources.fonts.{fontName}";

                using Stream? stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    return Results.NotFound($"Font '{fontName}' not found");
                }

                using MemoryStream memoryStream = new();
                stream.CopyTo(memoryStream);
                byte[] fontBytes = memoryStream.ToArray();

                string upperFontName = fontName.ToUpperInvariant();
                string contentType = upperFontName switch
                {
                    var name when name.EndsWith(".TTF", StringComparison.Ordinal) => "font/ttf",
                    var name when name.EndsWith(".WOFF", StringComparison.Ordinal) => "font/woff",
                    var name when name.EndsWith(".WOFF2", StringComparison.Ordinal) => "font/woff2",
                    var name when name.EndsWith(".EOT", StringComparison.Ordinal) => "application/vnd.ms-fontobject",
                    _ => "application/octet-stream"
                };

                return Results.File(fontBytes, contentType, fontName);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    title: "خطا در بارگذاری فونت",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithTags("Reports")
        .AllowAnonymous();
    }
}

