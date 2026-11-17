using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class OperationPreventsUnattendedExecutionException : ReportCatalogException
	{
		public OperationPreventsUnattendedExecutionException()
			: base(ErrorCode.rsOperationPreventsUnattendedExecution, ErrorStrings.rsOperationPreventsUnattendedExecution, null, null)
		{
		}
	}
}
