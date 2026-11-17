using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ScheduleDateTimeRangeException : ReportCatalogException
	{
		public ScheduleDateTimeRangeException()
			: base(ErrorCode.rsScheduleDateTimeRangeException, ErrorStrings.rsScheduleDateTimeRangeException, null, null)
		{
		}
	}
}
