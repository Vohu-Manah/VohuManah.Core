using ReportingServices.OnDemandProcessing;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.ReportProcessing
{
	internal sealed class FullOnDemandProcessingResult : OnDemandProcessingResult
	{
		private ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager m_chunkManager;

		private readonly bool m_snapshotChanged;

		public override bool SnapshotChanged => m_snapshotChanged;

		internal FullOnDemandProcessingResult(ReportingServices.ReportIntermediateFormat.ReportSnapshot newOdpSnapshot, ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager chunkManager, bool newOdpSnapshotChanged, IChunkFactory createChunkFactory, ParameterInfoCollection parameters, int autoRefresh, int numberOfPages, ProcessingMessageList warnings, bool eventInfoChanged, EventInformation newEventInfo, PaginationMode updatedPaginationMode, ReportProcessingFlags updatedProcessingFlags, UserProfileState usedUserProfileState, ExecutionLogContext executionLogContext)
			: base(createChunkFactory, newOdpSnapshot.DefinitionTreeHasDocumentMap, newOdpSnapshot.HasShowHide || newOdpSnapshot.HasUserSortFilter, parameters, autoRefresh, numberOfPages, warnings, eventInfoChanged, newEventInfo, updatedPaginationMode, updatedProcessingFlags, usedUserProfileState, executionLogContext)
		{
			m_chunkManager = chunkManager;
			m_snapshotChanged = newOdpSnapshotChanged;
		}

		public override void Save()
		{
			lock (this)
			{
				if (m_chunkManager != null)
				{
					m_chunkManager.SerializeSnapshot();
					m_chunkManager = null;
				}
			}
		}
	}
}
