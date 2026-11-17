using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ItemPathLengthExceededException : ReportCatalogException
	{
		public ItemPathLengthExceededException(string itemPath)
			: base(ErrorCode.rsItemPathLengthExceeded, ErrorStrings.rsItemPathLengthExceeded(itemPath, CatalogItemNameUtility.MaxItemPathLength), null, null)
		{
		}
	}
}
