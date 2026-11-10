using System;
using System.Security.Permissions;

namespace ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class ReportItems : MarshalByRefObject
	{
		public abstract ReportItem this[string key]
		{
			get;
		}
	}
}
