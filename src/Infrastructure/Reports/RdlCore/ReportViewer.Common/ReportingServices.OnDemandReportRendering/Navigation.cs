using ReportingServices.ReportIntermediateFormat;

namespace ReportingServices.OnDemandReportRendering
{
	internal abstract class Navigation
	{
		internal readonly ReportingServices.ReportIntermediateFormat.Navigation m_navigation;

		internal Navigation(ReportingServices.ReportIntermediateFormat.BandLayoutOptions bandLayout)
		{
			m_navigation = bandLayout.Navigation;
		}
	}
}
