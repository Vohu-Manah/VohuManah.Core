using ReportingServices.ReportIntermediateFormat;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class Coverflow : Navigation
	{
		private NavigationItem m_navigationItem;

		private Slider m_slider;

		public NavigationItem NavigationItem
		{
			get
			{
				if (m_navigationItem == null && RIFCoverflow.NavigationItem != null)
				{
					m_navigationItem = new NavigationItem(RIFCoverflow.NavigationItem);
				}
				return m_navigationItem;
			}
		}

		public Slider Slider
		{
			get
			{
				if (m_slider == null && RIFCoverflow.Slider != null)
				{
					m_slider = new Slider(RIFCoverflow.Slider);
				}
				return m_slider;
			}
		}

		private ReportingServices.ReportIntermediateFormat.Coverflow RIFCoverflow => m_navigation as ReportingServices.ReportIntermediateFormat.Coverflow;

		internal Coverflow(ReportingServices.ReportIntermediateFormat.BandLayoutOptions bandLayout)
			: base(bandLayout)
		{
		}
	}
}
