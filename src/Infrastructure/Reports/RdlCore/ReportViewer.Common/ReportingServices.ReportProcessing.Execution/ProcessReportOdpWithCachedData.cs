using ReportingServices.Diagnostics;
using ReportingServices.OnDemandProcessing;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace ReportingServices.ReportProcessing.Execution
{
	internal sealed class ProcessReportOdpWithCachedData : ProcessReportOdpInitial
	{
		private readonly OnDemandMetadata m_odpMetadataFromDataCache;

		protected override bool ProcessWithCachedData => true;

		public ProcessReportOdpWithCachedData(IConfiguration configuration, ProcessingContext pc, ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext, DateTime executionTime, OnDemandMetadata odpMetadataFromDataCache)
			: base(configuration, pc, report, errorContext, storeServerParameters, globalIDOwnerCollection, executionLogContext, executionTime)
		{
			Global.Tracer.Assert(odpMetadataFromDataCache != null, "Must provide existing metadata to process with cached data");
			m_odpMetadataFromDataCache = odpMetadataFromDataCache;
		}

		protected override OnDemandMetadata PrepareMetadata()
		{
			OnDemandMetadata onDemandMetadata = base.PrepareMetadata();
			onDemandMetadata.PrepareForCachedDataProcessing(m_odpMetadataFromDataCache);
			return onDemandMetadata;
		}
	}
}
