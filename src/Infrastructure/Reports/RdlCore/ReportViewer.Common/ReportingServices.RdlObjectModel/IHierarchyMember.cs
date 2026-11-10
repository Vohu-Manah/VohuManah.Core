using System.Collections.Generic;

namespace ReportingServices.RdlObjectModel
{
	internal interface IHierarchyMember
	{
		Group Group
		{
			get;
			set;
		}

		IList<SortExpression> SortExpressions
		{
			get;
			set;
		}

		IEnumerable<IHierarchyMember> Members
		{
			get;
		}
	}
}
