using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ScheduleAlreadyExists : ReportCatalogException
	{
		public ScheduleAlreadyExists(string name)
			: base(ErrorCode.rsScheduleAlreadyExists, ErrorStrings.rsScheduleAlreadyExists(name), null, null)
		{
		}
	}
}
