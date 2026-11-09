using System;
using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

namespace ReportViewer.Common.Renderers
{
    /// <summary>
    /// Provides a Linux-compatible PDF renderer based on PdfSharpCore.
    /// </summary>
    /// <remarks>
    /// This implementation acts as an adapter over PdfSharpCore so the rendering
    /// abstraction can remain platform-neutral while the actual document creation
    /// is delegated to a cross-platform library.
    /// </remarks>
    public class LinuxPdfRenderer : IPdfRenderer
    {
        /// <summary>
        /// Renders the supplied document content into a PDF stream.
        /// </summary>
        /// <param name="document">The document content to render.</param>
        /// <param name="output">The destination stream for the generated PDF.</param>
        public void RenderToPdf(object document, Stream output)
        {
            if (output == null) throw new ArgumentNullException(nameof(output));

            using (var pdf = new PdfDocument())
            {
                var page = pdf.AddPage();
                page.Width = XUnit.FromMillimeter(210);
                page.Height = XUnit.FromMillimeter(297);

                using (var gfx = XGraphics.FromPdfPage(page))
                {
                    var font = new XFont("Arial", 12);
                    string text = document?.ToString() ?? string.Empty;
                    gfx.DrawString(text, font, XBrushes.Black, 40, 40);
                }

                pdf.Save(output);
            }
        }
    }
}
