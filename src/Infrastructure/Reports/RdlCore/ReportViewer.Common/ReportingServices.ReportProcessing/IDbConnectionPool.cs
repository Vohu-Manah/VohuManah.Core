using ReportingServices.DataProcessing;

namespace ReportingServices.ReportProcessing
{
	internal interface IDbConnectionPool
	{
		int ConnectionCount
		{
			get;
		}

		IDbConnection GetConnection(ConnectionKey connectionKey);

		bool PoolConnection(IDbPoolableConnection connection, ConnectionKey connectionKey);

		void CloseConnections();
	}
}
