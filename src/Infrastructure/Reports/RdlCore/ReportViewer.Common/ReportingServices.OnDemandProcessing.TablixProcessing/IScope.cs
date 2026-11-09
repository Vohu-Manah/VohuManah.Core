using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal interface IScope : IStorable, IPersistable
	{
		bool TargetForNonDetailSort
		{
			get;
		}

		int[] SortFilterExpressionScopeInfoIndices
		{
			get;
		}

		int Depth
		{
			get;
		}

		IRIFReportScope RIFReportScope
		{
			get;
		}

		bool IsTargetForSort(int index, bool detailSort);

		bool TargetScopeMatched(int index, bool detailSort);

		void GetScopeValues(IReference<IHierarchyObj> targetScopeObj, List<object>[] scopeValues, ref int index);

		void CalculatePreviousAggregates();

		void ReadRow(DataActions dataAction, ITraversalContext context);

		bool InScope(string scope);

		IReference<IScope> GetOuterScope(bool includeSubReportContainingScope);

		string GetScopeName();

		int RecursiveLevel(string scope);

		void GetGroupNameValuePairs(Dictionary<string, object> pairs);

		void UpdateAggregates(AggregateUpdateContext context);

		void SetupEnvironment();
	}
}
