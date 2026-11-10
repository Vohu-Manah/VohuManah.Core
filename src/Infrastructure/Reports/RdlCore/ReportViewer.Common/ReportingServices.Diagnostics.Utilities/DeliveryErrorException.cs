using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class DeliveryErrorException : ReportCatalogException
	{
		public DeliveryErrorException(Exception innerException)
			: base(ErrorCode.rsDeliveryError, ErrorStrings.rsDeliverError, innerException, null)
		{
		}
	}
}
