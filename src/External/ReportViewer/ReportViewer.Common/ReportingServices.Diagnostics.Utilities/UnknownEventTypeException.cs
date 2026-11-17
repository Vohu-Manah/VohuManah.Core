using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class UnknownEventTypeException : ReportCatalogException
	{
		public UnknownEventTypeException(string eventType)
			: base(ErrorCode.rsUnknownEventType, ErrorStrings.rsUnknownEventType(eventType), null, null)
		{
		}
	}
}
