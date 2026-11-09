using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReadOnlyDataSetParameterException : ReportCatalogException
	{
		public ReadOnlyDataSetParameterException(string parameterName)
			: base(ErrorCode.rsReadOnlyDataSetParameter, ErrorStrings.rsReadOnlyDataSetParameter(parameterName), null, null)
		{
		}
	}
}
