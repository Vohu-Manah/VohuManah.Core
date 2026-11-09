using System;
using System.Security.Permissions;

namespace ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class ReportItem : MarshalByRefObject
	{
		public abstract object Value
		{
			get;
		}
	}
}
