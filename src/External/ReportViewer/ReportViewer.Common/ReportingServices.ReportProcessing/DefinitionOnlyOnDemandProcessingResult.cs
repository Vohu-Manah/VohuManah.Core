using ReportingServices.OnDemandProcessing;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.ReportProcessing
{
	internal sealed class DefinitionOnlyOnDemandProcessingResult : OnDemandProcessingResult
	{
		public override bool SnapshotChanged => false;

		internal DefinitionOnlyOnDemandProcessingResult(ReportingServices.ReportIntermediateFormat.ReportSnapshot newOdpSnapshot, ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager chunkManager, bool newOdpSnapshotChanged, IChunkFactory createChunkFactory, ParameterInfoCollection parameters, int autoRefresh, int numberOfPages, ProcessingMessageList warnings, bool eventInfoChanged, EventInformation newEventInfo, PaginationMode updatedPaginationMode, ReportProcessingFlags updatedProcessingFlags, UserProfileState usedUserProfileState, ExecutionLogContext executionLogContext)
			: base(createChunkFactory, newOdpSnapshot.DefinitionTreeHasDocumentMap, newOdpSnapshot.HasShowHide || newOdpSnapshot.HasUserSortFilter, parameters, autoRefresh, numberOfPages, warnings, eventInfoChanged, newEventInfo, updatedPaginationMode, updatedProcessingFlags, usedUserProfileState, executionLogContext)
		{
		}

		public override void Save()
		{
		}
	}
}
