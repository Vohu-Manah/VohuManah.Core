using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeDataTablixObjReference : RuntimeRDLDataRegionObjReference, IReference<RuntimeDataTablixObj>, IReference, IStorable, IPersistable, IOnDemandMemberOwnerInstanceReference, IReference<IOnDemandScopeInstance>, IReference<IOnDemandMemberOwnerInstance>
	{
		internal RuntimeDataTablixObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeDataTablixObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeDataTablixObj Value()
		{
			return (RuntimeDataTablixObj)InternalValue();
		}

		[DebuggerStepThrough]
		IOnDemandScopeInstance IReference<IOnDemandScopeInstance>.Value()
		{
			return (IOnDemandScopeInstance)InternalValue();
		}

		[DebuggerStepThrough]
		IOnDemandMemberOwnerInstance IReference<IOnDemandMemberOwnerInstance>.Value()
		{
			return (IOnDemandMemberOwnerInstance)InternalValue();
		}
	}
}
