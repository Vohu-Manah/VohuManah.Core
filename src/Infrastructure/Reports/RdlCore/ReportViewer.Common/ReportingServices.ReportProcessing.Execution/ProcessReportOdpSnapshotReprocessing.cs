using ReportingServices.Diagnostics;
using ReportingServices.OnDemandProcessing;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.ReportProcessing.Execution
{
	internal class ProcessReportOdpSnapshotReprocessing : ProcessReportOdpSnapshot
	{
		protected override bool ReprocessSnapshot => true;

		public ProcessReportOdpSnapshotReprocessing(IConfiguration configuration, ProcessingContext pc, ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext, OnDemandMetadata odpMetadataFromSnapshot)
			: base(configuration, pc, report, errorContext, storeServerParameters, globalIDOwnerCollection, executionLogContext, odpMetadataFromSnapshot)
		{
		}

		protected override OnDemandMetadata PrepareMetadata()
		{
			return new OnDemandMetadata(base.OdpMetadataFromSnapshot, base.ReportDefinition)
			{
				ReportSnapshot = new ReportingServices.ReportIntermediateFormat.ReportSnapshot(base.ReportDefinition, base.PublicProcessingContext.ReportContext.ItemName, base.PublicProcessingContext.Parameters, base.PublicProcessingContext.RequestUserName, base.OdpMetadataFromSnapshot.ReportSnapshot.ExecutionTime, base.PublicProcessingContext.ReportContext.HostRootUri, base.PublicProcessingContext.ReportContext.ParentPath, base.PublicProcessingContext.UserLanguage.Name)
			};
		}

		protected override void SetupReportLanguage(Merge odpMerge, ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			odpMerge.EvaluateReportLanguage(reportInstance, null);
		}
	}
}
