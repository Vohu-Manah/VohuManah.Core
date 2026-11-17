using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class CannotActivateSubscriptionException : ReportCatalogException
	{
		public CannotActivateSubscriptionException()
			: base(ErrorCode.rsCannotActivateSubscription, ErrorStrings.rsCannotActivateSubscription, null, null)
		{
		}
	}
}
