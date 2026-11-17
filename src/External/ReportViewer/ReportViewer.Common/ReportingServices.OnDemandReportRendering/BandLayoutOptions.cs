using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class BandLayoutOptions
	{
		private readonly ReportingServices.ReportIntermediateFormat.BandLayoutOptions m_bandLayoutDef;

		private Navigation m_navigation;

		public int RowCount => m_bandLayoutDef.RowCount;

		public int ColumnCount => m_bandLayoutDef.ColumnCount;

		public Navigation Navigation
		{
			get
			{
				if (m_navigation == null && m_bandLayoutDef.Navigation != null)
				{
					switch (m_bandLayoutDef.Navigation.GetObjectType())
					{
					case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Coverflow:
						m_navigation = new Coverflow(m_bandLayoutDef);
						break;
					case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PlayAxis:
						m_navigation = new PlayAxis(m_bandLayoutDef);
						break;
					case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Tabstrip:
						m_navigation = new Tabstrip(m_bandLayoutDef);
						break;
					default:
						Global.Tracer.Assert(false, "Unknown Band Navigation Type: {0}", m_bandLayoutDef.Navigation.GetObjectType());
						break;
					}
				}
				return m_navigation;
			}
		}

		internal BandLayoutOptions(ReportingServices.ReportIntermediateFormat.BandLayoutOptions bandLayoutDef)
		{
			m_bandLayoutDef = bandLayoutDef;
		}
	}
}
