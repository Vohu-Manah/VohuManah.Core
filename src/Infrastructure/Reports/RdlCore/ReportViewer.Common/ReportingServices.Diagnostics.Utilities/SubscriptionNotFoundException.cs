using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SubscriptionNotFoundException : ReportCatalogException
	{
		public SubscriptionNotFoundException(string idOrData)
			: base(ErrorCode.rsSubscriptionNotFound, ErrorStrings.rsSubscriptionNotFound(idOrData), null, null)
		{
		}
	}
}
