using System;
using Application.Library.Reports.Shared;

namespace Infrastructure.Reports.Html;

internal sealed class ReportTableGenerator : IReportTableGenerator
{
    private readonly Func<IHtmlBuilder> _htmlBuilderFactory;

    public ReportTableGenerator(Func<IHtmlBuilder> htmlBuilderFactory)
    {
        _htmlBuilderFactory = htmlBuilderFactory;
    }

    public string GenerateTable(ReportModel reportModel)
    {
        var htmlBuilder = _htmlBuilderFactory();

        htmlBuilder.AppendLine(ReportTableConstants.DivOpen);
        htmlBuilder.AppendLine(ReportTableConstants.H2Open + ReportTableConstants.ReportTitle + ReportTableConstants.H2Close);
        htmlBuilder.AppendLine(ReportTableConstants.TableOpen);

        BuildTableHeader(reportModel, htmlBuilder);
        BuildTableBody(reportModel, htmlBuilder);

        htmlBuilder.AppendLine(ReportTableConstants.TableClose);
        htmlBuilder.AppendLine(ReportTableConstants.DivClose);

        return htmlBuilder.ToString();
    }

    private static void BuildTableHeader(ReportModel reportModel, IHtmlBuilder htmlBuilder)
    {
        htmlBuilder.AppendLine(ReportTableConstants.TheadOpen);
        
        htmlBuilder.AppendLine("<tr" + ReportTableConstants.HeaderRowStyle + ">");

        object? firstItem = reportModel.DataSource.Cast<object>().FirstOrDefault();
        if (firstItem is not null)
        {
            
            htmlBuilder.AppendLine(ReportTableConstants.ThOpen + ReportTableConstants.RowNumberHeader + ReportTableConstants.ThClose);

            System.Reflection.PropertyInfo[] properties = firstItem.GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo prop in properties)
            {
                string title = ReportColumnTitles.Resolve(prop.Name);
                htmlBuilder.AppendLine(ReportTableConstants.ThOpen + System.Net.WebUtility.HtmlEncode(title) + ReportTableConstants.ThClose);
            }
        }
        else
        {
            htmlBuilder.AppendLine(ReportTableConstants.ThOpen + ReportTableConstants.ValueHeader + ReportTableConstants.ThClose);
            htmlBuilder.AppendLine(ReportTableConstants.ThOpen + ReportTableConstants.CountHeader + ReportTableConstants.ThClose);
        }

        htmlBuilder.AppendLine(ReportTableConstants.TrClose);
        htmlBuilder.AppendLine(ReportTableConstants.TheadClose);
    }

    private static void BuildTableBody(ReportModel reportModel, IHtmlBuilder htmlBuilder)
    {
        htmlBuilder.AppendLine(ReportTableConstants.TbodyOpen);

        int rowNumber = 1;
        foreach (object item in reportModel.DataSource)
        {
            htmlBuilder.AppendLine(ReportTableConstants.TrOpen);

            if (item != null)
            {
                
                htmlBuilder.AppendLine(ReportTableConstants.TdOpen + rowNumber.ToString(System.Globalization.CultureInfo.InvariantCulture) + ReportTableConstants.TdClose);

                
                
                
                
                
                System.Reflection.PropertyInfo[] properties = item.GetType().GetProperties();
                foreach (System.Reflection.PropertyInfo prop in properties)
                {
                    string value = prop.GetValue(item)?.ToString() ?? ReportTableConstants.EmptyString;
                    htmlBuilder.AppendLine(ReportTableConstants.TdOpen + System.Net.WebUtility.HtmlEncode(value) + ReportTableConstants.TdClose);
                }
            }

            htmlBuilder.AppendLine(ReportTableConstants.TrClose);
            rowNumber++;
        }

        htmlBuilder.AppendLine(ReportTableConstants.TbodyClose);
    }
}

