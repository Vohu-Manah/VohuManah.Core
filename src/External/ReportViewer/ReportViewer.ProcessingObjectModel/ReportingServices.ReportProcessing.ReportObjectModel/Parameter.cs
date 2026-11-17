using System;
using System.Security.Permissions;

namespace ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class Parameter : MarshalByRefObject
	{
		public abstract object Value
		{
			get;
		}

		public abstract object Label
		{
			get;
		}

		public abstract int Count
		{
			get;
		}

		public abstract bool IsMultiValue
		{
			get;
		}
	}
}
