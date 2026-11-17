using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections;
using System.Threading;

namespace ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class DataSourcesImpl : DataSources
	{
		private bool m_lockAdd;

		private Hashtable m_collection;

		public override ReportingServices.ReportProcessing.ReportObjectModel.DataSource this[string key]
		{
			get
			{
				if (key == null || m_collection == null)
				{
					throw new ReportProcessingException_NonExistingDataSourceReference(key);
				}
				try
				{
					ReportingServices.ReportProcessing.ReportObjectModel.DataSource dataSource = m_collection[key] as ReportingServices.ReportProcessing.ReportObjectModel.DataSource;
					if (dataSource == null)
					{
						throw new ReportProcessingException_NonExistingDataSourceReference(key);
					}
					return dataSource;
				}
				catch
				{
					throw new ReportProcessingException_NonExistingDataSourceReference(key);
				}
			}
		}

		internal DataSourcesImpl(int size)
		{
			m_lockAdd = (size > 1);
			m_collection = new Hashtable(size);
		}

		internal void Add(ReportingServices.ReportIntermediateFormat.DataSource dataSourceDef)
		{
			try
			{
				if (m_lockAdd)
				{
					Monitor.Enter(m_collection);
				}
				if (!m_collection.ContainsKey(dataSourceDef.Name))
				{
					m_collection.Add(dataSourceDef.Name, new DataSourceImpl(dataSourceDef));
				}
			}
			finally
			{
				if (m_lockAdd)
				{
					Monitor.Exit(m_collection);
				}
			}
		}
	}
}
