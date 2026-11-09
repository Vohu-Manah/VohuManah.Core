using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportServerServiceUnavailableException : ReportCatalogException
	{
		public ReportServerServiceUnavailableException(string serviceName)
			: base(ErrorCode.rsReportServerServiceUnavailable, ErrorStrings.rsReportServerServiceUnavailable(serviceName), null, null)
		{
		}
	}
}
