using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	internal sealed class ClaimsToWindowsTokenLoginTypeException : RSException
	{
		public ClaimsToWindowsTokenLoginTypeException()
			: base(ErrorCode.rsClaimsToWindowsTokenLoginTypeError, ErrorStrings.rsClaimsToWindowsTokenLoginTypeError, null, RSTrace.IsTraceInitialized ? RSTrace.WebServerTracer : null, null)
		{
		}
	}
}
