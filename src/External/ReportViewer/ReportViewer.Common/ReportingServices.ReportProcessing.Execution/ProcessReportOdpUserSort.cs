using ReportingServices.Diagnostics;
using ReportingServices.OnDemandProcessing;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.ReportProcessing.Execution
{
	internal class ProcessReportOdpUserSort : ProcessReportOdpSnapshotReprocessing
	{
		private readonly SortFilterEventInfoMap m_oldUserSortInformation;

		private readonly EventInformation m_newUserSortInformation;

		private readonly string m_oldUserSortEventSourceUniqueName;

		public ProcessReportOdpUserSort(IConfiguration configuration, ProcessingContext pc, ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext, OnDemandMetadata odpMetadataFromSnapshot, SortFilterEventInfoMap oldUserSortInformation, EventInformation newUserSortInformation, string oldUserSortEventSourceUniqueName)
			: base(configuration, pc, report, errorContext, storeServerParameters, globalIDOwnerCollection, executionLogContext, odpMetadataFromSnapshot)
		{
			m_oldUserSortInformation = oldUserSortInformation;
			m_newUserSortInformation = newUserSortInformation;
			m_oldUserSortEventSourceUniqueName = oldUserSortEventSourceUniqueName;
		}

		protected override void CompleteOdpContext(OnDemandProcessingContext odpContext)
		{
			odpContext.OldSortFilterEventInfo = m_oldUserSortInformation;
			odpContext.UserSortFilterInfo = m_newUserSortInformation;
			odpContext.UserSortFilterEventSourceUniqueName = m_oldUserSortEventSourceUniqueName;
		}

		protected override void PreProcessTablices(OnDemandProcessingContext odpContext, ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot)
		{
			Merge.PreProcessTablixes(base.ReportDefinition, odpContext, onlyWithSubReports: false);
			reportSnapshot.SortFilterEventInfo = odpContext.NewSortFilterEventInfo;
		}
	}
}
