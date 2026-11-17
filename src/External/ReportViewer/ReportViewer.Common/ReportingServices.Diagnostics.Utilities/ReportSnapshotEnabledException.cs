using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportSnapshotEnabledException : ReportCatalogException
	{
		public ReportSnapshotEnabledException()
			: base(ErrorCode.rsReportSnapshotEnabled, ErrorStrings.rsReportSnapshotEnabled, null, null)
		{
		}
	}
}
