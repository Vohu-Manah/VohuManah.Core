using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class CannotSubscribeToEventException : ReportCatalogException
	{
		public CannotSubscribeToEventException(string eventType)
			: base(ErrorCode.rsCannotSubscribeToEvent, ErrorStrings.rsCannotSubscribeToEvent(eventType), null, null)
		{
		}
	}
}
