using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System.Diagnostics;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeGroupObjReference : RuntimeHierarchyObjReference, IReference<RuntimeGroupObj>, IReference, IStorable, IPersistable, IReference<ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>
	{
		internal RuntimeGroupObjReference()
		{
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeGroupObj Value()
		{
			return (RuntimeGroupObj)InternalValue();
		}

		ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner IReference<ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>.Value()
		{
			return (ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner)InternalValue();
		}
	}
}
