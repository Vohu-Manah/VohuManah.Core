using System;
using System.Data;
using System.IO;
using ClosedXML.Excel;

namespace ReportViewer.Common.Renderers
{
    /// <summary>
    /// Provides a Linux-compatible Excel renderer based on ClosedXML.
    /// </summary>
    /// <remarks>
    /// This implementation acts as an adapter over ClosedXML so the rest of the
    /// rendering layer can depend on the cross-platform interface rather than the
    /// concrete library.
    /// </remarks>
    public class LinuxExcelRenderer : IExcelRenderer
    {
        /// <summary>
        /// Renders the supplied data into an Excel workbook and writes it to the output stream.
        /// </summary>
        /// <param name="data">The data payload to render.</param>
        /// <param name="output">The destination stream for the generated workbook.</param>
        public void RenderToExcel(object data, Stream output)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            using (var workbook = new XLWorkbook())
            {
                if (data is DataSet ds)
                {
                    foreach (DataTable table in ds.Tables)
                    {
                        var ws = workbook.Worksheets.Add(table.TableName ?? "Sheet1");
                        ws.Cell(1, 1).InsertTable(table);
                    }
                }
                else if (data is DataTable dt)
                {
                    var ws = workbook.Worksheets.Add(dt.TableName ?? "Sheet1");
                    ws.Cell(1, 1).InsertTable(dt);
                }
                else
                {
                    var ws = workbook.Worksheets.Add("Sheet1");
                    ws.Cell(1, 1).Value = data.ToString();
                }

                workbook.SaveAs(output);
            }
        }
    }
}
