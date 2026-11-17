using ReportingServices.ReportIntermediateFormat;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class MapFieldDefinition : MapObjectCollectionItem
	{
		private Map m_map;

		private ReportingServices.ReportIntermediateFormat.MapFieldDefinition m_defObject;

		public string Name => m_defObject.Name;

		public MapDataType DataType => m_defObject.DataType;

		internal Map MapDef => m_map;

		internal ReportingServices.ReportIntermediateFormat.MapFieldDefinition MapFieldDefinitionDef => m_defObject;

		public MapFieldDefinitionInstance Instance
		{
			get
			{
				if (m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new MapFieldDefinitionInstance(this);
				}
				return (MapFieldDefinitionInstance)m_instance;
			}
		}

		internal MapFieldDefinition(ReportingServices.ReportIntermediateFormat.MapFieldDefinition defObject, Map map)
		{
			m_defObject = defObject;
			m_map = map;
		}

		internal override void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
