using System;
using System.Security.Permissions;

namespace ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class Fields : MarshalByRefObject
	{
		public abstract Field this[string key]
		{
			get;
		}
	}
}
