using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class WrongItemTypeException : ReportCatalogException
	{
		public WrongItemTypeException(string itemPathOrType)
			: base(ErrorCode.rsWrongItemType, ErrorStrings.rsWrongItemType(itemPathOrType), null, null)
		{
		}
	}
}
