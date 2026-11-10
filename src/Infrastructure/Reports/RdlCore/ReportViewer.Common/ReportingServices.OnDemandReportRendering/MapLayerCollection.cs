using ReportingServices.ReportIntermediateFormat;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLayerCollection : MapObjectCollectionBase<MapLayer>
	{
		private Map m_map;

		public override int Count => m_map.MapDef.MapLayers.Count;

		internal MapLayerCollection(Map map)
		{
			m_map = map;
		}

		protected override MapLayer CreateMapObject(int index)
		{
			ReportingServices.ReportIntermediateFormat.MapLayer mapLayer = m_map.MapDef.MapLayers[index];
			if (mapLayer is ReportingServices.ReportIntermediateFormat.MapTileLayer)
			{
				return new MapTileLayer((ReportingServices.ReportIntermediateFormat.MapTileLayer)mapLayer, m_map);
			}
			if (mapLayer is ReportingServices.ReportIntermediateFormat.MapPolygonLayer)
			{
				return new MapPolygonLayer((ReportingServices.ReportIntermediateFormat.MapPolygonLayer)mapLayer, m_map);
			}
			if (mapLayer is ReportingServices.ReportIntermediateFormat.MapPointLayer)
			{
				return new MapPointLayer((ReportingServices.ReportIntermediateFormat.MapPointLayer)mapLayer, m_map);
			}
			if (mapLayer is ReportingServices.ReportIntermediateFormat.MapLineLayer)
			{
				return new MapLineLayer((ReportingServices.ReportIntermediateFormat.MapLineLayer)mapLayer, m_map);
			}
			return null;
		}
	}
}
