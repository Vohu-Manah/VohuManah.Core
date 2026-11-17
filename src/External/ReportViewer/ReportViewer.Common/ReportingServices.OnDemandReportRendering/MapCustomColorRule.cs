using ReportingServices.ReportIntermediateFormat;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class MapCustomColorRule : MapColorRule
	{
		private MapCustomColorCollection m_mapCustomColors;

		public MapCustomColorCollection MapCustomColors
		{
			get
			{
				if (m_mapCustomColors == null && MapCustomColorRuleDef.MapCustomColors != null)
				{
					m_mapCustomColors = new MapCustomColorCollection(this, m_map);
				}
				return m_mapCustomColors;
			}
		}

		internal ReportingServices.ReportIntermediateFormat.MapCustomColorRule MapCustomColorRuleDef => (ReportingServices.ReportIntermediateFormat.MapCustomColorRule)base.MapAppearanceRuleDef;

		public new MapCustomColorRuleInstance Instance => (MapCustomColorRuleInstance)GetInstance();

		internal MapCustomColorRule(ReportingServices.ReportIntermediateFormat.MapCustomColorRule defObject, MapVectorLayer mapVectorLayer, Map map)
			: base(defObject, mapVectorLayer, map)
		{
		}

		internal override MapAppearanceRuleInstance GetInstance()
		{
			if (m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new MapCustomColorRuleInstance(this);
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
			if (m_mapCustomColors != null)
			{
				m_mapCustomColors.SetNewContext();
			}
		}
	}
}
