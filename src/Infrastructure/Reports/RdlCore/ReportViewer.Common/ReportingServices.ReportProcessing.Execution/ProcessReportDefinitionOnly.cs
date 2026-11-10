using ReportingServices.Diagnostics;
using ReportingServices.OnDemandProcessing;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System;

namespace ReportingServices.ReportProcessing.Execution
{
	internal class ProcessReportDefinitionOnly : ProcessReportOdpInitial
	{
		protected override OnDemandProcessingContext.Mode OnDemandProcessingMode => OnDemandProcessingContext.Mode.DefinitionOnly;

		public ProcessReportDefinitionOnly(IConfiguration configuration, ProcessingContext pc, ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext, DateTime executionTime)
			: base(configuration, pc, report, errorContext, storeServerParameters, globalIDOwnerCollection, executionLogContext, executionTime)
		{
		}

		protected override OnDemandProcessingContext CreateOnDemandContext(OnDemandMetadata odpMetadata, ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot, UserProfileState initialUserDependency)
		{
			OnDemandProcessingContext onDemandProcessingContext = new OnDemandProcessingContext(base.PublicProcessingContext, base.ReportDefinition, odpMetadata, base.ErrorContext, reportSnapshot.ExecutionTime, base.StoreServerParameters, initialUserDependency, base.ExecutionLogContext, base.Configuration, GetAbortHelper());
			onDemandProcessingContext.ReportObjectModel.Initialize(base.PublicProcessingContext.Parameters);
			return onDemandProcessingContext;
		}

		protected override void CompleteOdpContext(OnDemandProcessingContext odpContext)
		{
			base.CompleteOdpContext(odpContext);
		}

		protected override ReportingServices.ReportIntermediateFormat.ReportInstance CreateReportInstance(OnDemandProcessingContext odpContext, OnDemandMetadata odpMetadata, ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot, out Merge odpMerge)
		{
			odpMerge = null;
			return null;
		}

		protected override void ResetEnvironment(OnDemandProcessingContext odpContext, ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
		}

		protected override void SetupReportLanguage(Merge odpMerge, ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
		}

		protected override void UpdateUserProfileLocation(OnDemandProcessingContext odpContext)
		{
		}

		protected override void PreProcessSnapshot(OnDemandProcessingContext odpContext, Merge odpMerge, ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot)
		{
			if (base.ReportDefinition.HasSubReports)
			{
				ReportProcessing.FetchSubReports(base.ReportDefinition, odpContext.ChunkFactory, odpContext.ErrorContext, odpContext.OdpMetadata, odpContext.ReportContext, odpContext.SubReportCallback, 0, odpContext.SnapshotProcessing, odpContext.ProcessWithCachedData, base.GlobalIDOwnerCollection, base.PublicProcessingContext.QueryParameters);
				SubReportInitializer.InitializeSubReportOdpContext(base.ReportDefinition, odpContext);
			}
		}
	}
}
