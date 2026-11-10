using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SubreportFromSnapshotException : ReportCatalogException
	{
		public SubreportFromSnapshotException()
			: base(ErrorCode.rsSubreportFromSnapshot, ErrorStrings.rsSubreportFromSnapshot, null, null)
		{
		}
	}
}
