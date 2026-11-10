using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	internal interface IActionOwner
	{
		Action Action
		{
			get;
		}

		List<string> FieldsUsedInValueExpression
		{
			get;
			set;
		}
	}
}
