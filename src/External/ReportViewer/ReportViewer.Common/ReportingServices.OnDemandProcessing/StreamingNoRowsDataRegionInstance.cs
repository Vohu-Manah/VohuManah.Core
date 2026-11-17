using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.OnDemandProcessing
{
	[PersistedWithinRequestOnly]
	[SkipStaticValidation]
	internal class StreamingNoRowsDataRegionInstance : StreamingNoRowsScopeInstanceBase, IOnDemandMemberOwnerInstance, IOnDemandScopeInstance, IStorable, IPersistable
	{
		public StreamingNoRowsDataRegionInstance(OnDemandProcessingContext odpContext, IRIFReportDataScope dataRegion)
			: base(odpContext, dataRegion)
		{
		}

		public IOnDemandMemberInstanceReference GetFirstMemberInstance(ReportHierarchyNode rifMember)
		{
			return null;
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.StreamingNoRowsDataRegionInstance;
		}
	}
}
