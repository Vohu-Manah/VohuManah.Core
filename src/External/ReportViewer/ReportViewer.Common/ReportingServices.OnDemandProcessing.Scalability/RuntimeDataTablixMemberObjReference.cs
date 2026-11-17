using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeDataTablixMemberObjReference : RuntimeMemberObjReference, IReference<RuntimeDataTablixMemberObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeDataTablixMemberObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeDataTablixMemberObjReference;
		}

		[DebuggerStepThrough]
		public RuntimeDataTablixMemberObj Value()
		{
			return (RuntimeDataTablixMemberObj)InternalValue();
		}
	}
}
