using System.IO;

namespace ReportViewer.Common.Renderers
{
    /// <summary>
    /// Defines the contract for rendering a document into a PDF stream.
    /// </summary>
    /// <remarks>
    /// Implementations are expected to serialize the supplied document content into
    /// a PDF document and write the bytes to the provided stream.
    /// </remarks>
    public interface IPdfRenderer
    {
        /// <summary>
        /// Renders the supplied document into a PDF stream.
        /// </summary>
        /// <param name="document">The document content to render.</param>
        /// <param name="output">The destination stream that receives the generated PDF.</param>
        void RenderToPdf(object document, Stream output);
    }
}
