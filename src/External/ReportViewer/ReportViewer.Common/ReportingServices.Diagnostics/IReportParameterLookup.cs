using System.Collections.Specialized;

namespace ReportingServices.Diagnostics
{
	internal interface IReportParameterLookup
	{
		string GetReportParamsInstanceId(NameValueCollection reportParameters);
	}
}
