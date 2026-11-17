using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SemanticQueryExtensionNotFoundException : ReportCatalogException
	{
		public SemanticQueryExtensionNotFoundException(string extension)
			: base(ErrorCode.rsSemanticQueryExtensionNotFound, ErrorStrings.rsSemanticQueryExtensionNotFound(extension), null, null)
		{
		}
	}
}
