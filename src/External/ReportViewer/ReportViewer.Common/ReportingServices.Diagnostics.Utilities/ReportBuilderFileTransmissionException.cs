using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReportBuilderFileTransmissionException : RSException
	{
		public ReportBuilderFileTransmissionException(Exception innerException, string fileName)
			: base(ErrorCode.rsReportBuilderFileTransmissionError, ErrorStrings.rsReportBuilderFileTransmissionError(fileName), innerException, RSTrace.IsTraceInitialized ? RSTrace.WebServerTracer : null, null)
		{
		}
	}
}
