namespace ReportingServices.ReportProcessing.ReportObjectModel
{
	internal sealed class DataSourceImpl : DataSource
	{
		private ReportingServices.ReportProcessing.DataSource m_dataSource;

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

		internal DataSourceImpl(ReportingServices.ReportProcessing.DataSource dataSourceDef)
		{
			m_dataSource = dataSourceDef;
		}
	}
}
