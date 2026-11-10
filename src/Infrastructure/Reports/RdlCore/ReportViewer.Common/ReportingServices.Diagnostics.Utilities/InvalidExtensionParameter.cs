using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidExtensionParameter : ReportCatalogException
	{
		public InvalidExtensionParameter(string reason)
			: base(ErrorCode.rsInvalidExtensionParameter, ErrorStrings.rsInvalidExtensionParameter(reason), null, null)
		{
		}
	}
}
