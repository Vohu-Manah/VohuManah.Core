using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	internal interface IInstancePath
	{
		List<InstancePathItem> InstancePath
		{
			get;
		}

		IInstancePath ParentInstancePath
		{
			get;
		}

		InstancePathItem InstancePathItem
		{
			get;
		}

		string UniqueName
		{
			get;
		}
	}
}
