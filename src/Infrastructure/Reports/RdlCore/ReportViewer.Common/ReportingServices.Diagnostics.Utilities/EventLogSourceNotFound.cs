using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class EventLogSourceNotFound : ReportCatalogException
	{
		public EventLogSourceNotFound(string source)
			: base(ErrorCode.rsEventLogSourceNotFound, ErrorStrings.rsEventLogSourceNotFound(source), null, null)
		{
		}
	}
}
