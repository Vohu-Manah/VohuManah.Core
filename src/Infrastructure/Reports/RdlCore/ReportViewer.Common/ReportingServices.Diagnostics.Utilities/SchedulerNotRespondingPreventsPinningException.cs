using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SchedulerNotRespondingPreventsPinningException : ReportCatalogException
	{
		public SchedulerNotRespondingPreventsPinningException()
			: base(ErrorCode.rsSchedulerNotResponding, ErrorStrings.rsSchedulerNotRespondingPreventsPinning, null, null)
		{
		}
	}
}
