using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeSortHierarchyObjReference : Reference<IHierarchyObj>, IReference<RuntimeSortHierarchyObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeSortHierarchyObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeSortHierarchyObjReference;
		}

		[DebuggerStepThrough]
		public RuntimeSortHierarchyObj Value()
		{
			return (RuntimeSortHierarchyObj)InternalValue();
		}
	}
}
