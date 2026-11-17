using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportNotReadyException : ReportCatalogException
	{
		public ReportNotReadyException()
			: base(ErrorCode.rsReportNotReady, ErrorStrings.rsReportNotReady, null, null)
		{
		}
	}
}
