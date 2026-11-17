using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing
{
	[PersistedWithinRequestOnly]
	[SkipStaticValidation]
	internal class StreamingNoRowsMemberInstance : StreamingNoRowsScopeInstanceBase, IOnDemandMemberInstance, IOnDemandMemberOwnerInstance, IOnDemandScopeInstance, IStorable, IPersistable
	{
		public List<object> GroupExprValues => null;

		public StreamingNoRowsMemberInstance(OnDemandProcessingContext odpContext, IRIFReportDataScope member)
			: base(odpContext, member)
		{
		}

		public IOnDemandMemberInstanceReference GetNextMemberInstance()
		{
			return null;
		}

		public IOnDemandScopeInstance GetCellInstance(IOnDemandMemberInstanceReference outerGroupInstanceRef, out IReference<IOnDemandScopeInstance> cellRef)
		{
			cellRef = null;
			return null;
		}

		public IOnDemandMemberInstanceReference GetFirstMemberInstance(ReportHierarchyNode rifMember)
		{
			return null;
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.StreamingNoRowsMemberInstance;
		}
	}
}
