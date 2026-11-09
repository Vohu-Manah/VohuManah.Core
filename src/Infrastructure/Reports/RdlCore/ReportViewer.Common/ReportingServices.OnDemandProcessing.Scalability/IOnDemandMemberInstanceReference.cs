using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IOnDemandMemberInstanceReference : IOnDemandMemberOwnerInstanceReference, IReference<IOnDemandScopeInstance>, IReference, IStorable, IPersistable, IReference<IOnDemandMemberOwnerInstance>, IReference<IOnDemandMemberInstance>
	{
	}
}
