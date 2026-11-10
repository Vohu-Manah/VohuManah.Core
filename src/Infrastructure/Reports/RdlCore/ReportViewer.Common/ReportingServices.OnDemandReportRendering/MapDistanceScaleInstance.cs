using ReportingServices.ReportIntermediateFormat;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class MapDistanceScaleInstance : MapDockableSubItemInstance
	{
		private MapDistanceScale m_defObject;

		private ReportColor m_scaleColor;

		private ReportColor m_scaleBorderColor;

		public ReportColor ScaleColor
		{
			get
			{
				if (m_scaleColor == null)
				{
					m_scaleColor = new ReportColor(((ReportingServices.ReportIntermediateFormat.MapDistanceScale)m_defObject.MapDockableSubItemDef).EvaluateScaleColor(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_scaleColor;
			}
		}

		public ReportColor ScaleBorderColor
		{
			get
			{
				if (m_scaleBorderColor == null)
				{
					m_scaleBorderColor = new ReportColor(((ReportingServices.ReportIntermediateFormat.MapDistanceScale)m_defObject.MapDockableSubItemDef).EvaluateScaleBorderColor(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return m_scaleBorderColor;
			}
		}

		internal MapDistanceScaleInstance(MapDistanceScale defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_scaleColor = null;
			m_scaleBorderColor = null;
		}
	}
}
