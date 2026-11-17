using System;
using System.Security.Permissions;

namespace ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public abstract class TextRun : MarshalByRefObject
	{
		public abstract object Value
		{
			get;
		}
	}
}
