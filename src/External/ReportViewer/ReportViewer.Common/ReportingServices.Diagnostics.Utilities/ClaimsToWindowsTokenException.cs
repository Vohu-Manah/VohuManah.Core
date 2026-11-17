using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	internal sealed class ClaimsToWindowsTokenException : RSException
	{
		public ClaimsToWindowsTokenException(Exception innerException)
			: base(ErrorCode.rsClaimsToWindowsTokenError, ErrorStrings.rsClaimsToWindowsTokenError, innerException, RSTrace.IsTraceInitialized ? RSTrace.WebServerTracer : null, null)
		{
		}
	}
}
