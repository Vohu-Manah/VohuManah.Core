using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportRendering;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalDataCell : DataCell
	{
		private ReportingServices.ReportIntermediateFormat.DataCell m_dataCellDef;

		public override DataValueCollection DataValues
		{
			get
			{
				if (m_dataValues == null)
				{
					m_dataValues = new DataValueCollection(m_dataCellDef, this, m_owner.RenderingContext, m_dataCellDef.DataValues, m_owner.Name, isChart: false);
				}
				return m_dataValues;
			}
		}

		internal override ReportingServices.ReportIntermediateFormat.DataCell DataCellDef => m_dataCellDef;

		internal override ReportingServices.ReportRendering.DataCell RenderItem => null;

		internal override IRIFReportScope RIFReportScope => m_dataCellDef;

		internal InternalDataCell(CustomReportItem owner, int rowIndex, int colIndex, ReportingServices.ReportIntermediateFormat.DataCell dataCellDef)
			: base(owner, rowIndex, colIndex)
		{
			m_dataCellDef = dataCellDef;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_dataCellDef != null)
			{
				m_dataCellDef.ClearStreamingScopeInstanceBinding();
			}
		}
	}
}
