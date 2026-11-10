using System;
using System.Text.Json;

namespace Infrastructure.Reports.Html;

internal sealed class ReportHtmlWrapper : IReportHtmlWrapper
{
    private readonly IReportResourceLoader _resourceLoader;
    private readonly Func<IHtmlBuilder> _htmlBuilderFactory;

    public ReportHtmlWrapper(
        IReportResourceLoader resourceLoader,
        Func<IHtmlBuilder> htmlBuilderFactory)
    {
        _resourceLoader = resourceLoader;
        _htmlBuilderFactory = htmlBuilderFactory;
    }

    public string Wrap(string reportHtml, string reportName, Dictionary<string, object> parameters, string orientation = "portrait")
    {
        var htmlBuilder = _htmlBuilderFactory();

        htmlBuilder.AppendLine(ReportConstants.Doctype);

        using (htmlBuilder.OpenTag(ReportConstants.HtmlTag, new Dictionary<string, string>
        {
            { ReportConstants.DirAttr, ReportConstants.HtmlDir },
            { ReportConstants.LangAttr, ReportConstants.HtmlLang }
        }))
        {
            BuildHead(reportName, orientation, htmlBuilder);
            BuildBody(reportHtml, reportName, parameters, htmlBuilder);
        }

        return htmlBuilder.ToString();
    }

    private void BuildHead(string reportName, string orientation, IHtmlBuilder htmlBuilder)
    {
        using (htmlBuilder.OpenTag(ReportConstants.HeadTag))
        {
            BuildMetaTags(orientation, htmlBuilder);
            BuildTitle(reportName, htmlBuilder);
            BuildStyles(htmlBuilder);
        }
    }

    private void BuildMetaTags(string orientation, IHtmlBuilder htmlBuilder)
    {
        htmlBuilder.SelfClosingTag(ReportConstants.MetaTag, new Dictionary<string, string>
        {
            { ReportConstants.CharsetAttr, ReportConstants.Charset }
        });

        htmlBuilder.SelfClosingTag(ReportConstants.MetaTag, new Dictionary<string, string>
        {
            { ReportConstants.NameAttr, ReportConstants.ViewportName },
            { ReportConstants.ContentAttr, ReportConstants.ViewportContent }
        });

        htmlBuilder.SelfClosingTag(ReportConstants.MetaTag, new Dictionary<string, string>
        {
            { "name", "report-orientation" },
            { ReportConstants.ContentAttr, orientation }
        });

        
        using (htmlBuilder.OpenTag(ReportConstants.StyleTag))
        {
            htmlBuilder.Raw($"@page {{ size: {orientation}; }}");
        }
    }

    private void BuildTitle(string reportName, IHtmlBuilder htmlBuilder)
    {
        using (htmlBuilder.OpenTag(ReportConstants.TitleTag))
        {
            htmlBuilder.Text(ReportConstants.ReportTitlePrefix + reportName);
        }
    }

    private void BuildStyles(IHtmlBuilder htmlBuilder)
    {
        using (htmlBuilder.OpenTag(ReportConstants.StyleTag))
        {
            
            string faSolid = Convert.ToBase64String(_resourceLoader.LoadFontAwesomeFont("fa-solid-900.woff2"));
            string faRegular = Convert.ToBase64String(_resourceLoader.LoadFontAwesomeFont("fa-regular-400.woff2"));
            string faBrands = Convert.ToBase64String(_resourceLoader.LoadFontAwesomeFont("fa-brands-400.woff2"));

            
            string fontAwesomeCss = _resourceLoader.LoadFontAwesomeCss();

            
            
            fontAwesomeCss = System.Text.RegularExpressions.Regex.Replace(
                fontAwesomeCss,
                @"url\(\.\./webfonts/fa-solid-900\.(woff2|ttf)\)",
                $"url('data:font/woff2;base64,{faSolid}')",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
            fontAwesomeCss = System.Text.RegularExpressions.Regex.Replace(
                fontAwesomeCss,
                @"url\(\.\./webfonts/fa-regular-400\.(woff2|ttf)\)",
                $"url('data:font/woff2;base64,{faRegular}')",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
            fontAwesomeCss = System.Text.RegularExpressions.Regex.Replace(
                fontAwesomeCss,
                @"url\(\.\./webfonts/fa-brands-400\.(woff2|ttf)\)",
                $"url('data:font/woff2;base64,{faBrands}')",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            
            htmlBuilder.Raw(fontAwesomeCss);
            htmlBuilder.Raw("\n");

            string shabnamRegular = Convert.ToBase64String(_resourceLoader.LoadFont("fonts.Shabnam-FD-WOL.woff2"));
            string shabnamBold = Convert.ToBase64String(_resourceLoader.LoadFont("fonts.Shabnam-Bold-FD-WOL.woff2"));

            htmlBuilder.Raw($@"
                            @font-face {{
                                font-family: 'Shabnam';
                                src: url('data:font/woff2;base64,{shabnamRegular}') format('woff2');
                                font-weight: 400;
                                font-style: normal;
                                font-display: swap;
                            }}

                            @font-face {{
                                font-family: 'Shabnam';
                                src: url('data:font/woff2;base64,{shabnamBold}') format('woff2');
                                font-weight: 700;
                                font-style: normal;
                                font-display: swap;
                            }}

                            @font-face {{
                                font-family: 'Lato';
                                src: url('https://fonts.gstatic.com/s/lato/v24/S6uyw4BMUTPHjx4wXg.woff2') format('woff2');
                                font-weight: 400;
                                font-style: normal;
                                font-display: swap;
                            }}

                            @font-face {{
                                font-family: 'Lato';
                                src: url('https://fonts.gstatic.com/s/lato/v24/S6u9w4BMUTPHh6UVSwiPHA.woff2') format('woff2');
                                font-weight: 700;
                                font-style: normal;
                                font-display: swap;
                            }}
                            ");
            htmlBuilder.Raw(_resourceLoader.LoadStyles());
        }
    }

    private void BuildBody(string reportHtml, string reportName, Dictionary<string, object> parameters, IHtmlBuilder htmlBuilder)
    {
        using (htmlBuilder.OpenTag(ReportConstants.BodyTag))
        {
            BuildToolbar(reportName, htmlBuilder);
            BuildReportContainer(reportHtml, htmlBuilder);
            BuildFooter(htmlBuilder);
            BuildScripts(reportName, parameters, htmlBuilder);
        }
    }

    private void BuildFooter(IHtmlBuilder htmlBuilder)
    {
        using (htmlBuilder.OpenTag(ReportConstants.DivTag, new Dictionary<string, string>
        {
            { ReportConstants.ClassAttr, ReportConstants.PaginationControlsClass }
        }))
        {

            BuildPaginationControls(htmlBuilder);

        }
    }

    private void BuildToolbar(string reportName, IHtmlBuilder htmlBuilder)
    {
        using (htmlBuilder.OpenTag(ReportConstants.DivTag, new Dictionary<string, string>
        {
            { ReportConstants.ClassAttr, ReportConstants.ToolbarClass }
        }))
        {
            BuildToolbarButtons(reportName, htmlBuilder);
            BuildZoomControls(htmlBuilder);
            BuildSearchControls(htmlBuilder);
            BuildDivider(htmlBuilder);
        }
    }


    private void BuildToolbarButtons(string reportName, IHtmlBuilder htmlBuilder)
    {
        using (htmlBuilder.OpenTag(ReportConstants.DivTag, new Dictionary<string, string>
        {
            { ReportConstants.ClassAttr, ReportConstants.ToolbarGroupClass }
        }))
        {
            BuildToolbarButton(
                $"{ReportConstants.ExportPdfFunction}('{reportName}', params)",
                ReportConstants.PdfIcon,
                ReportConstants.ExportPdfText,
                htmlBuilder);

            BuildToolbarButton(
                $"{ReportConstants.ExportExcelFunction}('{reportName}', params)",
                ReportConstants.ExcelIcon,
                ReportConstants.ExportExcelText,
                htmlBuilder);

            BuildToolbarButton(
                "printReport()",
                ReportConstants.PrintIcon,
                ReportConstants.PrintText,
                htmlBuilder);
        }
    }

    private void BuildToolbarButton(string onClick, string icon, string text, IHtmlBuilder htmlBuilder)
    {
        using (htmlBuilder.OpenTag(ReportConstants.ButtonTag, new Dictionary<string, string>
        {
            { ReportConstants.ClassAttr, ReportConstants.ToolbarBtnClass },
            { ReportConstants.OnclickAttr, onClick }
        }))
        {
            using (htmlBuilder.OpenTag(ReportConstants.SpanTag))
            {
                htmlBuilder.Raw(icon);
            }

            using (htmlBuilder.OpenTag(ReportConstants.SpanTag))
            {
                htmlBuilder.Text(text);
            }
        }
    }

    private void BuildDivider(IHtmlBuilder htmlBuilder)
    {
        
        
        htmlBuilder.Raw($"<{ReportConstants.DivTag} {ReportConstants.ClassAttr}=\"{ReportConstants.DividerClass}\"></{ReportConstants.DivTag}>");
    }

    private void BuildSearchControls(IHtmlBuilder htmlBuilder)
    {
        using (htmlBuilder.OpenTag(ReportConstants.DivTag, new Dictionary<string, string>
        {
            { ReportConstants.ClassAttr, ReportConstants.SearchControlsClass },
            { ReportConstants.IdAttr, "searchControls" }
        }))
        {
            
            htmlBuilder.SelfClosingTag(ReportConstants.InputTag, new Dictionary<string, string>
            {
                { ReportConstants.TypeAttr, "text" },
                { ReportConstants.IdAttr, "searchInput" },
                { ReportConstants.ClassAttr, ReportConstants.SearchInputClass },
                { ReportConstants.PlaceholderAttr, "جستجوی کلمه در گزارش..." },
                { ReportConstants.OnkeyupAttr, "handleSearchKeyup(event)" }
            });

            
            using (htmlBuilder.OpenTag(ReportConstants.ButtonTag, new Dictionary<string, string>
            {
                { ReportConstants.ClassAttr, ReportConstants.SearchBtnClass },
                { ReportConstants.OnclickAttr, "performSearch()" }
            }))
            {
                htmlBuilder.Raw("<i class=\"fas fa-search\"></i>");
            }

            
            using (htmlBuilder.OpenTag(ReportConstants.ButtonTag, new Dictionary<string, string>
            {
                { ReportConstants.ClassAttr, ReportConstants.SearchBtnClass },
                { ReportConstants.IdAttr, "clearSearchBtn" },
                { ReportConstants.OnclickAttr, "clearSearch()" },
                { ReportConstants.StyleAttr, "display:none;" }
            }))
            {
                htmlBuilder.Raw("<i class=\"fas fa-times\"></i>");
            }

            

            
            using (htmlBuilder.OpenTag(ReportConstants.ButtonTag, new Dictionary<string, string>
            {
                { ReportConstants.ClassAttr, ReportConstants.SearchNavBtnClass },
                { ReportConstants.IdAttr, "prevResultBtn" },
                { ReportConstants.OnclickAttr, "goToPreviousResult()" },
                { ReportConstants.StyleAttr, "display:none;" }
            }))
            {
                htmlBuilder.Raw("<i class=\"fas fa-chevron-up\"></i>");
            }

            
            using (htmlBuilder.OpenTag(ReportConstants.ButtonTag, new Dictionary<string, string>
            {
                { ReportConstants.ClassAttr, ReportConstants.SearchNavBtnClass },
                { ReportConstants.IdAttr, "nextResultBtn" },
                { ReportConstants.OnclickAttr, "goToNextResult()" },
                { ReportConstants.StyleAttr, "display:none;" }
            }))
            {
                htmlBuilder.Raw("<i class=\"fas fa-chevron-down\"></i>");
            }
        }
    }

    private void BuildPaginationControls(IHtmlBuilder htmlBuilder)
    {
        using (htmlBuilder.OpenTag(ReportConstants.DivTag, new Dictionary<string, string>
        {
            { ReportConstants.ClassAttr, ReportConstants.PaginationControlsClass },
            { ReportConstants.IdAttr, "paginationControls" }
        }))
        {
            
            using (htmlBuilder.OpenTag(ReportConstants.ButtonTag, new Dictionary<string, string>
            {
                { ReportConstants.ClassAttr, ReportConstants.PaginationBtnClass },
                { ReportConstants.OnclickAttr, "goToFirstPage()" }
            }))
            {
                htmlBuilder.Raw("<i class=\"fas fa-angle-double-right\"></i>");
            }

            
            using (htmlBuilder.OpenTag(ReportConstants.ButtonTag, new Dictionary<string, string>
            {
                { ReportConstants.ClassAttr, ReportConstants.PaginationBtnClass },
                { ReportConstants.OnclickAttr, "goToPreviousPage()" }
            }))
            {
                htmlBuilder.Raw("<i class=\"fas fa-angle-right\"></i>");
            }

            
            using (htmlBuilder.OpenTag(ReportConstants.SpanTag, new Dictionary<string, string>
            {
                { ReportConstants.ClassAttr, ReportConstants.PaginationInfoClass }
            }))
            {
                htmlBuilder.Text("صفحه 1 از 1");
            }

            
            using (htmlBuilder.OpenTag(ReportConstants.ButtonTag, new Dictionary<string, string>
            {
                { ReportConstants.ClassAttr, ReportConstants.PaginationBtnClass },
                { ReportConstants.OnclickAttr, "goToNextPage()" }
            }))
            {
                htmlBuilder.Raw("<i class=\"fas fa-angle-left\"></i>");
            }

            
            using (htmlBuilder.OpenTag(ReportConstants.ButtonTag, new Dictionary<string, string>
            {
                { ReportConstants.ClassAttr, ReportConstants.PaginationBtnClass },
                { ReportConstants.OnclickAttr, "goToLastPage()" }
            }))
            {
                htmlBuilder.Raw("<i class=\"fas fa-angle-double-left\"></i>");
            }
        }
    }

    private void BuildZoomControls(IHtmlBuilder htmlBuilder)
    {
        using (htmlBuilder.OpenTag(ReportConstants.DivTag, new Dictionary<string, string>
        {
            { ReportConstants.ClassAttr, ReportConstants.ZoomControlsClass }
        }))
        {
            BuildZoomButton(ReportConstants.ZoomOutFunction, ReportConstants.ZoomOutSymbol, htmlBuilder);
            BuildZoomValue(htmlBuilder);
            BuildZoomButton(ReportConstants.ZoomInFunction, ReportConstants.ZoomInSymbol, htmlBuilder);
            BuildZoomButton(ReportConstants.ResetZoomFunction, ReportConstants.ResetZoomSymbol, htmlBuilder);
        }
    }

    private void BuildZoomButton(string onClick, string text, IHtmlBuilder htmlBuilder)
    {
        using (htmlBuilder.OpenTag(ReportConstants.ButtonTag, new Dictionary<string, string>
        {
            { ReportConstants.ClassAttr, ReportConstants.ZoomBtnClass },
            { ReportConstants.OnclickAttr, $"{onClick}()" }
        }))
        {
            htmlBuilder.Raw(text);
        }
    }

    private void BuildZoomValue(IHtmlBuilder htmlBuilder)
    {
        using (htmlBuilder.OpenTag(ReportConstants.SpanTag, new Dictionary<string, string>
        {
            { ReportConstants.ClassAttr, ReportConstants.ZoomValueClass },
            { ReportConstants.IdAttr, ReportConstants.ZoomValueId }
        }))
        {
            htmlBuilder.Text(ReportConstants.DefaultZoomValue);
        }
    }

    private void BuildReportContainer(string reportHtml, IHtmlBuilder htmlBuilder)
    {
        using (htmlBuilder.OpenTag(ReportConstants.DivTag, new Dictionary<string, string>
        {
            { ReportConstants.ClassAttr, ReportConstants.ReportContainerClass }
        }))
        {
            using (htmlBuilder.OpenTag(ReportConstants.DivTag, new Dictionary<string, string>
            {
                { ReportConstants.ClassAttr, ReportConstants.ReportContentClass },
                { ReportConstants.IdAttr, ReportConstants.ReportContentId }
            }))
            {
                htmlBuilder.Raw(reportHtml);
            }
        }
    }

    private void BuildScripts(string reportName, Dictionary<string, object> parameters, IHtmlBuilder htmlBuilder)
    {
        using (htmlBuilder.OpenTag(ReportConstants.ScriptTag))
        {
            BuildScriptContent(reportName, parameters, htmlBuilder);
        }
    }

    private void BuildScriptContent(string reportName, Dictionary<string, object> parameters, IHtmlBuilder htmlBuilder)
    {
        
        
        
        
        var paramsJson = JsonSerializer.Serialize(parameters)
            .Replace("<", "\\u003c", StringComparison.Ordinal)
            .Replace(">", "\\u003e", StringComparison.Ordinal)
            .Replace("&", "\\u0026", StringComparison.Ordinal)
            .Replace("\u2028", "\\u2028", StringComparison.Ordinal)
            .Replace("\u2029", "\\u2029", StringComparison.Ordinal);

        
        var safeReportName = reportName
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("'", "\\'", StringComparison.Ordinal);

        htmlBuilder.Raw($@"
        const reportName = '{safeReportName}';
        const params = {paramsJson};
        
        {_resourceLoader.LoadScripts()}");
    }
}
