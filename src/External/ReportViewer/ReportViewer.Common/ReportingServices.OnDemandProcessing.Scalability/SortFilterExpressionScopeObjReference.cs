using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class SortFilterExpressionScopeObjReference : Reference<IHierarchyObj>, IReference<RuntimeSortFilterEventInfo.SortFilterExpressionScopeObj>, IReference, IStorable, IPersistable
	{
		internal SortFilterExpressionScopeObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.SortFilterExpressionScopeObjReference;
		}

		[DebuggerStepThrough]
		public RuntimeSortFilterEventInfo.SortFilterExpressionScopeObj Value()
		{
			return (RuntimeSortFilterEventInfo.SortFilterExpressionScopeObj)InternalValue();
		}
	}
}
