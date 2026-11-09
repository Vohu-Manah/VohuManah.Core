using ReportingServices.ReportIntermediateFormat;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class MapField : MapObjectCollectionItem
	{
		private Map m_map;

		private ReportingServices.ReportIntermediateFormat.MapField m_defObject;

		public string Name => m_defObject.Name;

		public string Value => m_defObject.Value;

		internal Map MapDef => m_map;

		internal ReportingServices.ReportIntermediateFormat.MapField MapFieldDef => m_defObject;

		public MapFieldInstance Instance
		{
			get
			{
				if (m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new MapFieldInstance(this);
				}
				return (MapFieldInstance)m_instance;
			}
		}

		internal MapField(ReportingServices.ReportIntermediateFormat.MapField defObject, Map map)
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
