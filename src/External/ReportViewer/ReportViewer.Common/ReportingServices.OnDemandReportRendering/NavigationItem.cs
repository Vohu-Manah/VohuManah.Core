using ReportingServices.ReportIntermediateFormat;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class NavigationItem
	{
		private readonly ReportingServices.ReportIntermediateFormat.NavigationItem m_navigationItem;

		public string ReportItemReference => m_navigationItem.ReportItemReference;

		public ReportItem ReportItem => null;

		internal NavigationItem(ReportingServices.ReportIntermediateFormat.NavigationItem navigationItem)
		{
			m_navigationItem = navigationItem;
		}
	}
}
