using ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Security.Permissions;

namespace ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public abstract class Scope : MarshalByRefObject
	{
		public abstract Fields Fields
		{
			get;
		}
	}
}
