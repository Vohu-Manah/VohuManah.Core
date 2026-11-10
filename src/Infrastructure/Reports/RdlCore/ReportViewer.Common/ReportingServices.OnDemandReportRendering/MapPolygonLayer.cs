using ReportingServices.ReportIntermediateFormat;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class MapPolygonLayer : MapVectorLayer
	{
		private MapPolygonTemplate m_mapPolygonTemplate;

		private MapPolygonRules m_mapPolygonRules;

		private MapPointTemplate m_mapCenterPointTemplate;

		private MapPointRules m_mapcenterPointRules;

		private MapPolygonCollection m_mapPolygons;

		public MapPolygonTemplate MapPolygonTemplate
		{
			get
			{
				if (m_mapPolygonTemplate == null && MapPolygonLayerDef.MapPolygonTemplate != null)
				{
					m_mapPolygonTemplate = new MapPolygonTemplate(MapPolygonLayerDef.MapPolygonTemplate, this, m_map);
				}
				return m_mapPolygonTemplate;
			}
		}

		public MapPolygonRules MapPolygonRules
		{
			get
			{
				if (m_mapPolygonRules == null && MapPolygonLayerDef.MapPolygonRules != null)
				{
					m_mapPolygonRules = new MapPolygonRules(MapPolygonLayerDef.MapPolygonRules, this, m_map);
				}
				return m_mapPolygonRules;
			}
		}

		public MapPointTemplate MapCenterPointTemplate
		{
			get
			{
				if (m_mapCenterPointTemplate == null)
				{
					ReportingServices.ReportIntermediateFormat.MapPointTemplate mapCenterPointTemplate = MapPolygonLayerDef.MapCenterPointTemplate;
					if (mapCenterPointTemplate != null && mapCenterPointTemplate is ReportingServices.ReportIntermediateFormat.MapMarkerTemplate)
					{
						m_mapCenterPointTemplate = new MapMarkerTemplate((ReportingServices.ReportIntermediateFormat.MapMarkerTemplate)mapCenterPointTemplate, this, m_map);
					}
				}
				return m_mapCenterPointTemplate;
			}
		}

		public MapPointRules MapCenterPointRules
		{
			get
			{
				if (m_mapcenterPointRules == null && MapPolygonLayerDef.MapCenterPointRules != null)
				{
					m_mapcenterPointRules = new MapPointRules(MapPolygonLayerDef.MapCenterPointRules, this, m_map);
				}
				return m_mapcenterPointRules;
			}
		}

		public MapPolygonCollection MapPolygons
		{
			get
			{
				if (m_mapPolygons == null && MapPolygonLayerDef.MapPolygons != null)
				{
					m_mapPolygons = new MapPolygonCollection(this, m_map);
				}
				return m_mapPolygons;
			}
		}

		internal ReportingServices.ReportIntermediateFormat.MapPolygonLayer MapPolygonLayerDef => (ReportingServices.ReportIntermediateFormat.MapPolygonLayer)base.MapLayerDef;

		public new MapPolygonLayerInstance Instance => (MapPolygonLayerInstance)GetInstance();

		internal MapPolygonLayer(ReportingServices.ReportIntermediateFormat.MapPolygonLayer defObject, Map map)
			: base(defObject, map)
		{
		}

		internal override MapLayerInstance GetInstance()
		{
			if (m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new MapPolygonLayerInstance(this);
			}
			return (MapVectorLayerInstance)m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_mapPolygonTemplate != null)
			{
				m_mapPolygonTemplate.SetNewContext();
			}
			if (m_mapPolygonRules != null)
			{
				m_mapPolygonRules.SetNewContext();
			}
			if (m_mapCenterPointTemplate != null)
			{
				m_mapCenterPointTemplate.SetNewContext();
			}
			if (m_mapcenterPointRules != null)
			{
				m_mapcenterPointRules.SetNewContext();
			}
			if (m_mapPolygons != null)
			{
				m_mapPolygons.SetNewContext();
			}
		}
	}
}
