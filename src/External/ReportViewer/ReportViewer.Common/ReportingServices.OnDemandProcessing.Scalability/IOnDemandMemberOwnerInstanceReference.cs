using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IOnDemandMemberOwnerInstanceReference : IReference<IOnDemandScopeInstance>, IReference, IStorable, IPersistable, IReference<IOnDemandMemberOwnerInstance>
	{
	}
}
