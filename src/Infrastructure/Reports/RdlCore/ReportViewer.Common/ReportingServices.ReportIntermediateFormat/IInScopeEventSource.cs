using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	internal interface IInScopeEventSource : IReferenceable, IGloballyReferenceable, IGlobalIDOwner
	{
		ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get;
		}

		string Name
		{
			get;
		}

		ReportItem Parent
		{
			get;
		}

		EndUserSort UserSort
		{
			get;
		}

		List<InstancePathItem> InstancePath
		{
			get;
		}

		GroupingList ContainingScopes
		{
			get;
			set;
		}

		string Scope
		{
			get;
			set;
		}

		bool IsTablixCellScope
		{
			get;
			set;
		}

		bool IsDetailScope
		{
			get;
			set;
		}

		bool IsSubReportTopLevelScope
		{
			get;
			set;
		}

		InitializationContext.ScopeChainInfo ScopeChainInfo
		{
			get;
			set;
		}

		List<int> GetPeerSortFilters(bool create);

		InScopeSortFilterHashtable GetSortFiltersInScope(bool create, bool inDetail);
	}
}
