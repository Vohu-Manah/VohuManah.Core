using ReportingServices.ReportIntermediateFormat;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class ScalePinInstance : TickMarkStyleInstance
	{
		private double? m_location;

		private bool? m_enable;

		public double Location
		{
			get
			{
				if (!m_location.HasValue)
				{
					m_location = ((ReportingServices.ReportIntermediateFormat.ScalePin)m_defObject.TickMarkStyleDef).EvaluateLocation(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_location.Value;
			}
		}

		public bool Enable
		{
			get
			{
				if (!m_enable.HasValue)
				{
					m_enable = ((ReportingServices.ReportIntermediateFormat.ScalePin)m_defObject.TickMarkStyleDef).EvaluateEnable(ReportScopeInstance, m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return m_enable.Value;
			}
		}

		internal ScalePinInstance(ScalePin defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_location = null;
			m_enable = null;
		}
	}
}
