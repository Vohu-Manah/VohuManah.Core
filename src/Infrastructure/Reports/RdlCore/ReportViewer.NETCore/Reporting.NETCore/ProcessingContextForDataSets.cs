using ReportingServices;
using ReportingServices.DataExtensions;
using ReportingServices.Diagnostics;
using ReportingServices.Interfaces;
using ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Security.Principal;
using System.Threading;

namespace Reporting.NETCore
{
	internal class ProcessingContextForDataSets : ProcessingContext
	{
		private IEnumerable m_dataSources;

		private LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback m_subReportInfoCallback;

		internal override bool EnableDataBackedParameters => false;

		internal override IProcessingDataExtensionConnection CreateAndSetupDataExtensionFunction => new DataSetExtensionConnection(m_subReportInfoCallback, m_dataSources);

		internal override RuntimeDataSourceInfoCollection DataSources => null;

		internal override RuntimeDataSetInfoCollection SharedDataSetReferences => null;

		internal override bool CanShareDataSets => false;

		public ProcessingContextForDataSets(PreviewItemContext reportContext, ParameterInfoCollection parameters, IEnumerable dataSources, ReportProcessing.OnDemandSubReportCallback subReportCallback, LocalDataRetrievalFromDataSet.GetSubReportDataSetCallback subReportInfoCallback, IGetResource getResourceFunction, IChunkFactory chunkFactory, ReportRuntimeSetup reportRuntimeSetup, CreateAndRegisterStream createStreamCallback)
			: base(reportContext, Environment.UserName, parameters, subReportCallback, getResourceFunction, chunkFactory, ReportProcessing.ExecutionType.Live, Thread.CurrentThread.CurrentCulture, UserProfileState.Both, UserProfileState.None, reportRuntimeSetup, createStreamCallback, isHistorySnapshot: false, new ViewerJobContextImpl(), new ViewerExtensionFactory(), DataProtectionLocal.Instance)
		{
			m_dataSources = dataSources;
			m_subReportInfoCallback = subReportInfoCallback;
		}

		internal override ReportProcessing.ProcessingContext CreateInternalProcessingContext(string chartName, ReportingServices.ReportProcessing.Report report, ErrorContext errorContext, DateTime executionTime, UserProfileState allowUserProfileState, bool isHistorySnapshot, bool snapshotProcessing, bool processWithCachedData, ReportProcessing.GetReportChunk getChunkCallback, ReportProcessing.CreateReportChunk cacheDataCallback)
		{
			ReportingServices.ReportProcessing.Global.Tracer.Assert(condition: false, "CreateInternalProcessingContext is not used for ODP Engine Controls");
			return null;
		}

		internal override ReportProcessing.ProcessingContext ParametersInternalProcessingContext(ErrorContext errorContext, DateTime executionTimeStamp, bool isSnapshot)
		{
			ReportingServices.ReportProcessing.Global.Tracer.Assert(condition: false, "ParametersInternalProcessingContext is not used for ODP Engine Controls");
			return null;
		}
	}
}
