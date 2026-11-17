using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ScheduleNotFoundException : ReportCatalogException
	{
		public ScheduleNotFoundException(string idOrData)
			: base(ErrorCode.rsScheduleNotFound, ErrorStrings.rsScheduleNotFound(idOrData), null, null)
		{
		}
	}
}
