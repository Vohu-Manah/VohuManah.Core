using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ItemAlreadyExistsException : ReportCatalogException
	{
		public ItemAlreadyExistsException(string itemPath)
			: base(ErrorCode.rsItemAlreadyExists, ErrorStrings.rsItemAlreadyExists(itemPath), null, null)
		{
		}
	}
}
