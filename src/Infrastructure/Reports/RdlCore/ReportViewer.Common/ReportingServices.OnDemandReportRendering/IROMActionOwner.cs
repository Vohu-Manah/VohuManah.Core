using System.Collections.Generic;

namespace ReportingServices.OnDemandReportRendering
{
	internal interface IROMActionOwner
	{
		string UniqueName
		{
			get;
		}

		List<string> FieldsUsedInValueExpression
		{
			get;
		}
	}
}
