using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeDetailObjReference : RuntimeHierarchyObjReference, IReference<RuntimeDetailObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeDetailObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeDetailObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeDetailObj Value()
		{
			return (RuntimeDetailObj)InternalValue();
		}
	}
}
