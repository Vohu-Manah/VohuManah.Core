using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportParameterValueNotSetException : ReportCatalogException
	{
		public ReportParameterValueNotSetException(string parameterName)
			: base(ErrorCode.rsReportParameterValueNotSet, ErrorStrings.rsReportParameterValueNotSet(parameterName), null, null)
		{
		}
	}
}
