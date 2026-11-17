using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeHierarchyObjReference : RuntimeDataRegionObjReference, IReference<IHierarchyObj>, IReference, IStorable, IPersistable, IReference<RuntimeHierarchyObj>
	{
		internal RuntimeHierarchyObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeHierarchyObjReference;
		}

		[DebuggerStepThrough]
		IHierarchyObj IReference<IHierarchyObj>.Value()
		{
			return (IHierarchyObj)InternalValue();
		}

		[DebuggerStepThrough]
		public new RuntimeHierarchyObj Value()
		{
			return (RuntimeHierarchyObj)InternalValue();
		}
	}
}
