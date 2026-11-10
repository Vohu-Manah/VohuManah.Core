using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidReportLinkException : ReportCatalogException
	{
		public InvalidReportLinkException()
			: base(ErrorCode.rsInvalidReportLink, ErrorStrings.rsInvalidReportLink, null, null)
		{
		}
	}
}
