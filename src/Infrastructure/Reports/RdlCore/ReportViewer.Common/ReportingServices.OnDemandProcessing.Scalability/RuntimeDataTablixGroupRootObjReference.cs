using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeDataTablixGroupRootObjReference : RuntimeGroupRootObjReference, IReference<RuntimeDataTablixGroupRootObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeDataTablixGroupRootObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeDataTablixGroupRootObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeDataTablixGroupRootObj Value()
		{
			return (RuntimeDataTablixGroupRootObj)InternalValue();
		}
	}
}
