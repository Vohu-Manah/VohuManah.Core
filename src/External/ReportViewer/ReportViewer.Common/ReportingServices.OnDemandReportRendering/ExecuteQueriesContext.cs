using ReportingServices.DataExtensions;
using ReportingServices.DataProcessing;
using ReportingServices.Diagnostics;
using ReportingServices.Interfaces;
using ReportingServices.ReportProcessing;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class ExecuteQueriesContext
	{
		private readonly IDbConnection m_connection;

		private readonly IProcessingDataExtensionConnection m_dataExtensionConnection;

		private readonly DataSourceInfo m_dataSourceInfo;

		private readonly CreateAndRegisterStream m_createAndRegisterStream;

		private readonly IJobContext m_jobContext;

		internal IDbConnection Connection => m_connection;

		internal CreateAndRegisterStream CreateAndRegisterStream => m_createAndRegisterStream;

		internal IJobContext JobContext => m_jobContext;

		internal ExecuteQueriesContext(IDbConnection connection, IProcessingDataExtensionConnection dataExtensionConnection, DataSourceInfo dataSourceInfo, CreateAndRegisterStream createAndRegisterStream, IJobContext jobContext)
		{
			m_connection = connection;
			m_dataExtensionConnection = dataExtensionConnection;
			m_dataSourceInfo = dataSourceInfo;
			m_createAndRegisterStream = createAndRegisterStream;
			m_jobContext = jobContext;
		}

		internal IDbCommand CreateCommandWrapperForCancel(IDbCommand command)
		{
			return new CommandWrappedForCancel(command, m_dataExtensionConnection, null, m_dataSourceInfo, null, m_connection);
		}
	}
}
