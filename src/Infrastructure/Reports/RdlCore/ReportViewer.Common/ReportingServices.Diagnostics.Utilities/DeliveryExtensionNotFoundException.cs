using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class DeliveryExtensionNotFoundException : ReportCatalogException
	{
		public DeliveryExtensionNotFoundException()
			: base(ErrorCode.rsDeliveryExtensionNotFound, ErrorStrings.rsDeliveryExtensionNotFound, null, null)
		{
		}
	}
}
