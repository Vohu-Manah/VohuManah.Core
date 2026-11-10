using ReportingServices.ReportIntermediateFormat;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeImage : GaugePanelItem
	{
		internal ReportingServices.ReportIntermediateFormat.GaugeImage GaugeImageDef => (ReportingServices.ReportIntermediateFormat.GaugeImage)m_defObject;

		public new GaugeImageInstance Instance => (GaugeImageInstance)GetInstance();

		internal GaugeImage(ReportingServices.ReportIntermediateFormat.GaugeImage defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

		internal override BaseInstance GetInstance()
		{
			if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new GaugeImageInstance(this);
			}
			return m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
