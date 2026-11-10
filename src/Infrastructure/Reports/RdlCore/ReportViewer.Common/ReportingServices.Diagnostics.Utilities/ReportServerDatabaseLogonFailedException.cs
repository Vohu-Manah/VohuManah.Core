using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportServerDatabaseLogonFailedException : ReportCatalogException
	{
		public ReportServerDatabaseLogonFailedException(Exception innerException)
			: base(ErrorCode.rsReportServerDatabaseLogonFailed, ErrorStrings.rsReportServerDatabaseLogonFailed, innerException, null)
		{
		}
	}
}
