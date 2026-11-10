using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeDataRegionObjReference : IScopeReference, IReference<RuntimeDataRegionObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeDataRegionObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeDataRegionObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeDataRegionObj Value()
		{
			return (RuntimeDataRegionObj)InternalValue();
		}
	}
}
