using ReportingServices.ReportRendering;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class ParagraphFilteredStyle : Style
	{
		internal ParagraphFilteredStyle(ReportingServices.ReportRendering.ReportItem renderReportItem, RenderingContext renderingContext, bool useRenderStyle)
			: base(renderReportItem, renderingContext, useRenderStyle)
		{
		}

		protected override bool IsAvailableStyle(StyleAttributeNames styleName)
		{
			if (styleName == StyleAttributeNames.TextAlign || styleName == StyleAttributeNames.LineHeight)
			{
				return true;
			}
			return false;
		}
	}
}
