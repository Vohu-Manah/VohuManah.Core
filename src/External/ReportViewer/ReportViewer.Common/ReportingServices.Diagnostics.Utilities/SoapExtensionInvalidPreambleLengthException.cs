using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SoapExtensionInvalidPreambleLengthException : RSException
	{
		public SoapExtensionInvalidPreambleLengthException(long length)
			: base(ErrorCode.rsSoapExtensionInvalidPreambleLengthError, ErrorStrings.rsSoapExtensionInvalidPreambleLengthError(length.ToString(CultureInfo.CurrentCulture)), null, RSTrace.IsTraceInitialized ? RSTrace.WebServerTracer : null, null)
		{
		}

		public SoapExtensionInvalidPreambleLengthException(string length)
			: base(ErrorCode.rsSoapExtensionInvalidPreambleLengthError, ErrorStrings.rsSoapExtensionInvalidPreambleLengthError(length), null, RSTrace.IsTraceInitialized ? RSTrace.WebServerTracer : null, null)
		{
		}
	}
}
