using ReportingServices.ReportIntermediateFormat;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class Field
	{
		private ReportingServices.ReportIntermediateFormat.Field m_fieldDef;

		public string Name => m_fieldDef.Name;

		public string DataField => m_fieldDef.DataField;

		internal Field(ReportingServices.ReportIntermediateFormat.Field fieldDef)
		{
			m_fieldDef = fieldDef;
		}
	}
}
