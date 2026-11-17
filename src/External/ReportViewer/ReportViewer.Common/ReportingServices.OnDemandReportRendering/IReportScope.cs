using ReportingServices.ReportIntermediateFormat;

namespace ReportingServices.OnDemandReportRendering
{
	internal interface IReportScope
	{
		IReportScopeInstance ReportScopeInstance
		{
			get;
		}

		IRIFReportScope RIFReportScope
		{
			get;
		}
	}
}
