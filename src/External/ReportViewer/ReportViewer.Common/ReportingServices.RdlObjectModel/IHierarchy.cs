using System.Collections.Generic;

namespace ReportingServices.RdlObjectModel
{
	internal interface IHierarchy
	{
		IEnumerable<IHierarchyMember> Members
		{
			get;
		}
	}
}
