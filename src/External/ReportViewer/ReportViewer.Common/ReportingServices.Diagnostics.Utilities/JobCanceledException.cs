using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class JobCanceledException : ReportCatalogException
	{
		public JobCanceledException(Exception innerException)
			: base(ErrorCode.rsJobWasCanceled, ErrorStrings.rsJobWasCanceled, innerException, null)
		{
		}
	}
}
