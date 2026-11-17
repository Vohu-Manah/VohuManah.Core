using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class UnknownDataSetParameterException : ReportCatalogException
	{
		public UnknownDataSetParameterException(string parameterName)
			: base(ErrorCode.rsUnknownDataSetParameter, ErrorStrings.rsUnknownDataSetParameter(parameterName), null, null)
		{
		}
	}
}
