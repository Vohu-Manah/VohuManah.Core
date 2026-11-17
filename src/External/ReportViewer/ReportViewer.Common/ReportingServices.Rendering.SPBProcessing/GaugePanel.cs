using ReportingServices.OnDemandReportRendering;
using ReportingServices.Rendering.RPLProcessing;

namespace ReportingServices.Rendering.SPBProcessing
{
	internal sealed class GaugePanel : DynamicImage
	{
		protected override PaginationInfoItems PaginationInfoEnum => PaginationInfoItems.GaugePanel;

		protected override bool SpecialBorderHandling => false;

		protected override PageBreak PageBreak => ((ReportingServices.OnDemandReportRendering.GaugePanel)m_source).PageBreak;

		protected override string PageName => ((GaugePanelInstance)m_source.Instance).PageName;

		internal GaugePanel(ReportingServices.OnDemandReportRendering.GaugePanel source, PageContext pageContext, bool createForRepeat)
			: base(source, pageContext, createForRepeat)
		{
		}

		protected override byte GetElementToken(PageContext pageContext)
		{
			return 14;
		}

		protected override string GenerateStreamName(PageContext pageContext)
		{
			return pageContext.GenerateStreamName((GaugePanelInstance)m_source.Instance);
		}

		protected override RPLItem CreateRPLItem()
		{
			return new RPLGaugePanel();
		}
	}
}
