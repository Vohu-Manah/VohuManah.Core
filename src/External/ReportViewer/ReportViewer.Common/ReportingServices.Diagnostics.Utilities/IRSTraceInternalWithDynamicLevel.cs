using System.Diagnostics;

namespace ReportingServices.Diagnostics.Utilities
{
	internal interface IRSTraceInternalWithDynamicLevel : IRSTraceInternal
	{
		void SetTraceLevel(TraceLevel traceLevel);
	}
}
