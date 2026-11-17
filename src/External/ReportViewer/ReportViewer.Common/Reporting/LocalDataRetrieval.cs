using ReportingServices.DataExtensions;
using ReportingServices.Diagnostics;
using ReportingServices.Interfaces;
using ReportingServices.ReportProcessing;
using System.Collections;

namespace Reporting
{
	internal abstract class LocalDataRetrieval
	{
		public abstract bool SupportsQueries
		{
			get;
		}

		public abstract ProcessingContext CreateProcessingContext(PreviewItemContext itemContext, ParameterInfoCollection parameters, IEnumerable dataSources, RuntimeDataSourceInfoCollection dataSourceInfoColl, RuntimeDataSetInfoCollection dataSetInfoColl, SharedDataSetCompiler sharedDataSetCompiler, DatasourceCredentialsCollection credentials, ReportProcessing.OnDemandSubReportCallback subReportCallback, IGetResource getResourceFunction, IChunkFactory chunkFactory, ReportRuntimeSetup runtimeSetup, CreateAndRegisterStream createStreamCallback);
	}
}
