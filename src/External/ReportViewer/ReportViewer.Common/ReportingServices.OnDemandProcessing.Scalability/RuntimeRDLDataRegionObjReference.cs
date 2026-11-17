using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System.Diagnostics;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeRDLDataRegionObjReference : RuntimeDataRegionObjReference, IReference<IHierarchyObj>, IReference, IStorable, IPersistable, IReference<RuntimeRDLDataRegionObj>, IReference<IDataRowSortOwner>, IReference<ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>, IReference<IDataCorrelation>
	{
		internal RuntimeRDLDataRegionObjReference()
		{
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRDLDataRegionObjReference;
		}

		[DebuggerStepThrough]
		IHierarchyObj IReference<IHierarchyObj>.Value()
		{
			return (IHierarchyObj)InternalValue();
		}

		[DebuggerStepThrough]
		public new RuntimeRDLDataRegionObj Value()
		{
			return (RuntimeRDLDataRegionObj)InternalValue();
		}

		[DebuggerStepThrough]
		IDataRowSortOwner IReference<IDataRowSortOwner>.Value()
		{
			return (IDataRowSortOwner)InternalValue();
		}

		[DebuggerStepThrough]
		ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner IReference<ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner>.Value()
		{
			return (ReportingServices.ReportProcessing.ReportProcessing.IFilterOwner)InternalValue();
		}

		[DebuggerStepThrough]
		IDataCorrelation IReference<IDataCorrelation>.Value()
		{
			return (IDataCorrelation)InternalValue();
		}
	}
}
