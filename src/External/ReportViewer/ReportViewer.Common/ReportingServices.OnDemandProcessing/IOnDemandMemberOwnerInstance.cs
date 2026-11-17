using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.OnDemandProcessing
{
	internal interface IOnDemandMemberOwnerInstance : IOnDemandScopeInstance, IStorable, IPersistable
	{
		IOnDemandMemberInstanceReference GetFirstMemberInstance(ReportHierarchyNode rifMember);
	}
}
