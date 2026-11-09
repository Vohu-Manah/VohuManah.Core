using ReportingServices.ReportIntermediateFormat;

namespace ReportingServices.OnDemandReportRendering
{
	internal abstract class MapSpatialData
	{
		protected Map m_map;

		protected MapVectorLayer m_mapVectorLayer;

		private ReportingServices.ReportIntermediateFormat.MapSpatialData m_defObject;

		protected MapSpatialDataInstance m_instance;

		internal Map MapDef => m_map;

		internal ReportingServices.ReportIntermediateFormat.MapSpatialData MapSpatialDataDef => m_defObject;

		internal MapSpatialDataInstance Instance => GetInstance();

		internal MapSpatialData(MapVectorLayer mapVectorLayer, Map map)
		{
			m_defObject = mapVectorLayer.MapVectorLayerDef.MapSpatialData;
			m_mapVectorLayer = mapVectorLayer;
			m_map = map;
		}

		internal abstract MapSpatialDataInstance GetInstance();

		internal virtual void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
