using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportTimeoutExpiredException : ReportCatalogException
	{
		public ReportTimeoutExpiredException(Exception innerException)
			: base(ErrorCode.rsReportTimeoutExpired, ErrorStrings.rsReportTimeoutExpired, innerException, null)
		{
		}
	}
}
