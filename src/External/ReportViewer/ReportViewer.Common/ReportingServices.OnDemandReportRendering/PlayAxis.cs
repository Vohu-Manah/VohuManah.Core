using ReportingServices.ReportIntermediateFormat;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class PlayAxis : Navigation
	{
		public Slider Slider
		{
			get
			{
				if (RIFPlayAxis.Slider == null)
				{
					return null;
				}
				return new Slider(RIFPlayAxis.Slider);
			}
		}

		public DockingOption DockingOption => RIFPlayAxis.DockingOption;

		private ReportingServices.ReportIntermediateFormat.PlayAxis RIFPlayAxis => m_navigation as ReportingServices.ReportIntermediateFormat.PlayAxis;

		internal PlayAxis(ReportingServices.ReportIntermediateFormat.BandLayoutOptions bandLayout)
			: base(bandLayout)
		{
		}
	}
}
