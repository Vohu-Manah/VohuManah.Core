using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	internal sealed class UnhandledHttpApplicationException : RSException
	{
		public UnhandledHttpApplicationException(Exception innerException)
			: base(ErrorCode.rsUnhandledHttpApplicationError, ErrorStrings.rsUnhandledHttpApplicationError, innerException, RSTrace.IsTraceInitialized ? RSTrace.WebServerTracer : null, null)
		{
		}
	}
}
