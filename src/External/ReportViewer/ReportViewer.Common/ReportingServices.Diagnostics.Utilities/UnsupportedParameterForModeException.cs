using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class UnsupportedParameterForModeException : ReportCatalogException
	{
		public UnsupportedParameterForModeException(string mode, string parameterName)
			: base(ErrorCode.rsUnsupportedParameterForMode, ErrorStrings.rsUnsupportedParameterForMode(mode, parameterName), null, null)
		{
		}
	}
}
