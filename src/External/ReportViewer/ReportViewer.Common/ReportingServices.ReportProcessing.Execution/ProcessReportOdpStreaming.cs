using ReportingServices.Diagnostics;
using ReportingServices.OnDemandProcessing;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace ReportingServices.ReportProcessing.Execution
{
	internal class ProcessReportOdpStreaming : ProcessReportOdpInitial
	{
		private readonly IAbortHelper m_abortHelper;

		protected override OnDemandProcessingContext.Mode OnDemandProcessingMode => OnDemandProcessingContext.Mode.Streaming;

		public ProcessReportOdpStreaming(IConfiguration configuration, ProcessingContext pc, ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext, DateTime executionTime, IAbortHelper abortHelper)
			: base(configuration, pc, report, errorContext, storeServerParameters, globalIDOwnerCollection, executionLogContext, executionTime)
		{
			m_abortHelper = abortHelper;
		}

		protected override void PreProcessSnapshot(OnDemandProcessingContext odpContext, Merge odpMerge, ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot)
		{
			SetupInitialOdpState(odpContext, reportInstance, reportSnapshot);
		}

		protected override IAbortHelper GetAbortHelper()
		{
			return m_abortHelper ?? base.GetAbortHelper();
		}

		protected override void CleanupAbortHandler(OnDemandProcessingContext odpContext)
		{
		}
	}
}
