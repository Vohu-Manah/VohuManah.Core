using System;

namespace ReportingServices.OnDemandReportRendering
{
	internal interface IPageBreakItem
	{
		[Obsolete("Use PageBreak.BreakLocation instead.")]
		PageBreakLocation PageBreakLocation
		{
			get;
		}

		PageBreak PageBreak
		{
			get;
		}
	}
}
