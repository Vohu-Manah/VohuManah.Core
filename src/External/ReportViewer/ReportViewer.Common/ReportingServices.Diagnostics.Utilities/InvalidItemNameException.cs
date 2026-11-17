using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidItemNameException : ReportCatalogException
	{
		public InvalidItemNameException(string invalidName, int maxItemNameLength)
			: base(ErrorCode.rsInvalidItemName, ErrorStrings.rsInvalidItemName(invalidName, maxItemNameLength), null, null)
		{
		}
	}
}
