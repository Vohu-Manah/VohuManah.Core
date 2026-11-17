using ReportingServices.ReportIntermediateFormat;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class Slider
	{
		private readonly ReportingServices.ReportIntermediateFormat.Slider m_slider;

		public bool Hidden => m_slider.Hidden;

		public LabelData LabelData
		{
			get
			{
				if (m_slider.LabelData == null)
				{
					return null;
				}
				return new LabelData(m_slider.LabelData);
			}
		}

		internal Slider(ReportingServices.ReportIntermediateFormat.Slider slider)
		{
			m_slider = slider;
		}
	}
}
