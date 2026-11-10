using System.IO;

namespace ReportViewer.Common.Renderers
{
    /// <summary>
    /// Defines the contract for rendering data into an Excel workbook stream.
    /// </summary>
    /// <remarks>
    /// Implementations should accept a data payload, such as a DataTable or DataSet,
    /// and write the resulting workbook to the supplied output stream.
    /// </remarks>
    public interface IExcelRenderer
    {
        /// <summary>
        /// Renders the supplied data into an Excel workbook and writes it to the output stream.
        /// </summary>
        /// <param name="data">The data payload to render, such as a DataTable or DataSet.</param>
        /// <param name="output">The destination stream that receives the generated workbook.</param>
        void RenderToExcel(object data, Stream output);
    }
}
