using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ExecutionNotFoundException : ReportCatalogException
	{
		public ExecutionNotFoundException(string ExecutionID)
			: base(ErrorCode.rsExecutionNotFound, ErrorStrings.rsExecutionNotFound(ExecutionID), null, null)
		{
		}
	}
}
