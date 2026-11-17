using ReportingServices.DataExtensions;
using ReportingServices.DataProcessing;
using ReportingServices.Diagnostics;
using System;

namespace ReportingServices.ReportProcessing
{
	internal interface IProcessingDataExtensionConnection
	{
		bool MustResolveSharedDataSources
		{
			get;
		}

		void DataSetRetrieveForReportInstance(ICatalogItemContext itemContext, ParameterInfoCollection reportParameters);

		IDbConnection OpenDataSourceExtensionConnection(IProcessingDataSource dataSource, string connectionString, DataSourceInfo dataSourceInfo, string datasetName);

		void HandleImpersonation(IProcessingDataSource dataSource, DataSourceInfo dataSourceInfo, string datasetName, IDbConnection connection, System.Action afterImpersonationAction);

		void CloseConnectionWithoutPool(IDbConnection connection);

		void CloseConnection(IDbConnection connection, IProcessingDataSource dataSource, DataSourceInfo dataSourceInfo);
	}
}
