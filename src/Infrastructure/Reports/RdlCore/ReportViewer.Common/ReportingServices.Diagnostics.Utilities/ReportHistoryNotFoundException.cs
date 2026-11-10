using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportHistoryNotFoundException : ReportCatalogException
	{
		public ReportHistoryNotFoundException(string reportPath, string snapshotId)
			: base(ErrorCode.rsReportHistoryNotFound, ErrorStrings.rsReportHistoryNotFound(reportPath, snapshotId), null, null)
		{
		}
	}
}
