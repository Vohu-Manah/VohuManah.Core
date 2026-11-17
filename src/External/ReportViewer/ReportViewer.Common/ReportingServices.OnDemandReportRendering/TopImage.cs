using ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class TopImage : BaseGaugeImage
	{
		private ReportColorProperty m_hueColor;

		public ReportColorProperty HueColor
		{
			get
			{
				if (m_hueColor == null)
				{
					ExpressionInfo hueColor = TopImageDef.HueColor;
					if (hueColor != null)
					{
						m_hueColor = new ReportColorProperty(hueColor.IsExpression, hueColor.OriginalText, hueColor.IsExpression ? null : new ReportColor(hueColor.StringValue.Trim(), allowTransparency: true), hueColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
					}
				}
				return m_hueColor;
			}
		}

		internal ReportingServices.ReportIntermediateFormat.TopImage TopImageDef => (ReportingServices.ReportIntermediateFormat.TopImage)m_defObject;

		public new TopImageInstance Instance => (TopImageInstance)GetInstance();

		internal TopImage(ReportingServices.ReportIntermediateFormat.TopImage defObject, GaugePanel gaugePanel)
			: base(defObject, gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

		internal override BaseGaugeImageInstance GetInstance()
		{
			if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new TopImageInstance(this);
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
