using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	internal sealed class FileSizeException : ReportCatalogException
	{
		public FileSizeException()
			: base(ErrorCode.rsDataCacheMismatch, ErrorStrings.rsFileSize, null, null)
		{
		}
	}
}
