using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing
{
	internal interface IOnDemandMemberInstance : IOnDemandMemberOwnerInstance, IOnDemandScopeInstance, IStorable, IPersistable
	{
		List<object> GroupExprValues
		{
			get;
		}

		IOnDemandMemberInstanceReference GetNextMemberInstance();

		IOnDemandScopeInstance GetCellInstance(IOnDemandMemberInstanceReference outerGroupInstanceRef, out IReference<IOnDemandScopeInstance> cellRef);
	}
}
