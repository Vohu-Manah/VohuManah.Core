using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportServerHttpRuntimeException : ReportCatalogException
	{
		public ReportServerHttpRuntimeException(Exception innerException, string appDomain, string additionalTraceMessage)
			: base(ErrorCode.rsHttpRuntimeError, ErrorStrings.rsHttpRuntimeError(appDomain), innerException, additionalTraceMessage)
		{
		}

		public ReportServerHttpRuntimeException(string appDomain, string additionalTraceMessage)
			: base(ErrorCode.rsHttpRuntimeError, ErrorStrings.rsHttpRuntimeError(appDomain), null, additionalTraceMessage)
		{
		}
	}
}
