using ReportingServices.Diagnostics;
using ReportingServices.OnDemandProcessing;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace ReportingServices.ReportProcessing.Execution
{
	internal sealed class ProcessDataShape : ProcessReportOdpStreaming
	{
		private readonly bool m_useParallelQueryExecution;

		public ProcessDataShape(IConfiguration configuration, ProcessingContext pc, ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext, DateTime executionTime, IAbortHelper abortHelper, bool useParallelQueryExecution)
			: base(configuration, pc, report, errorContext, storeServerParameters, globalIDOwnerCollection, executionLogContext, executionTime, abortHelper)
		{
			m_useParallelQueryExecution = useParallelQueryExecution;
		}

		protected override void PreProcessSnapshot(OnDemandProcessingContext odpContext, Merge odpMerge, ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot)
		{
			ParallelPreloadQueries(odpContext);
			SetupInitialOdpState(odpContext, reportInstance, reportSnapshot);
		}

		private void ParallelPreloadQueries(OnDemandProcessingContext odpContext)
		{
			if (m_useParallelQueryExecution)
			{
				_ = odpContext.JobContext;
			}
		}
	}
}
