using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidModelDrillthroughReportException : ReportCatalogException
	{
		public InvalidModelDrillthroughReportException(string reportName)
			: base(ErrorCode.rsInvalidModelDrillthroughReport, ErrorStrings.rsInvalidModelDrillthroughReport(reportName), null, null)
		{
		}
	}
}
