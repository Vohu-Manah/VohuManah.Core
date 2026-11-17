using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReadOnlyReportParameterException : ReportCatalogException
	{
		public ReadOnlyReportParameterException(string parameterName)
			: base(ErrorCode.rsReadOnlyReportParameter, ErrorStrings.rsReadOnlyReportParameter(parameterName), null, null)
		{
		}
	}
}
