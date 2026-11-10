using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System.Diagnostics;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeOnDemandDataSetObjReference : IScopeReference, IReference<IHierarchyObj>, IReference, IStorable, IPersistable, IReference<RuntimeOnDemandDataSetObj>, IReference<ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>
	{
		internal RuntimeOnDemandDataSetObjReference()
		{
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeOnDemandDataSetObjReference;
		}

		[DebuggerStepThrough]
		IHierarchyObj IReference<IHierarchyObj>.Value()
		{
			return (IHierarchyObj)InternalValue();
		}

		[DebuggerStepThrough]
		public new RuntimeOnDemandDataSetObj Value()
		{
			return (RuntimeOnDemandDataSetObj)InternalValue();
		}

		[DebuggerStepThrough]
		ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner IReference<ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>.Value()
		{
			return (ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner)InternalValue();
		}
	}
}
