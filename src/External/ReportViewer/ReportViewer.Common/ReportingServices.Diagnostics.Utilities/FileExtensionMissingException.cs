using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class FileExtensionMissingException : ReportCatalogException
	{
		public FileExtensionMissingException()
			: base(ErrorCode.rsFileExtensionRequired, ErrorStrings.rsFileExtensionRequired, null, null)
		{
		}
	}
}
