using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportProcessing.ReportObjectModel;

namespace ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class DataSourceImpl : ReportingServices.ReportProcessing.ReportObjectModel.DataSource
	{
		private ReportingServices.ReportIntermediateFormat.DataSource m_dataSource;

		public override string DataSourceReference
		{
			get
			{
				if (m_dataSource.SharedDataSourceReferencePath == null)
				{
					return m_dataSource.DataSourceReference;
				}
				return m_dataSource.SharedDataSourceReferencePath;
			}
		}

		public override string Type => m_dataSource.Type;

		internal DataSourceImpl(ReportingServices.ReportIntermediateFormat.DataSource dataSourceDef)
		{
			m_dataSource = dataSourceDef;
		}
	}
}
