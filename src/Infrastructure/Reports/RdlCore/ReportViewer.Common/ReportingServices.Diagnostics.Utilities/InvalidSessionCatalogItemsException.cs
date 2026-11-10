using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidSessionCatalogItemsException : ReportCatalogException
	{
		public InvalidSessionCatalogItemsException(Exception innerException, string errorString)
			: base(ErrorCode.rsInvalidSessionCatalogItems, ErrorStrings.rsInvalidSessionCatalogItems(errorString), innerException, null)
		{
		}
	}
}
