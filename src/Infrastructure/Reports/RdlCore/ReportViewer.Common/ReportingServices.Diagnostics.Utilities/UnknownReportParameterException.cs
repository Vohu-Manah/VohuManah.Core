using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class UnknownReportParameterException : ReportCatalogException
	{
		public UnknownReportParameterException(string parameterName)
			: base(ErrorCode.rsUnknownReportParameter, ErrorStrings.rsUnknownReportParameter(parameterName), null, null)
		{
		}
	}
}
