using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class DataExtensionNotFoundException : ReportCatalogException
	{
		public DataExtensionNotFoundException(string extension)
			: base(ErrorCode.rsDataExtensionNotFound, ErrorStrings.rsDataExtensionNotFound(extension), null, null)
		{
		}
	}
}
