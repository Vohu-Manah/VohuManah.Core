using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportSnapshotNotEnabledException : ReportCatalogException
	{
		public ReportSnapshotNotEnabledException()
			: base(ErrorCode.rsReportSnapshotNotEnabled, ErrorStrings.rsReportSnapshotNotEnabled, null, null)
		{
		}
	}
}
