using System;
using System.Security.Permissions;

namespace ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public abstract class TextRuns : MarshalByRefObject
	{
		public abstract TextRun this[int index]
		{
			get;
		}
	}
}
