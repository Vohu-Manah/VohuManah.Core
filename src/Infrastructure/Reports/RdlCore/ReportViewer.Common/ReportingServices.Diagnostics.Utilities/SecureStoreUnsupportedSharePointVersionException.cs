using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	internal sealed class SecureStoreUnsupportedSharePointVersionException : ReportCatalogException
	{
		public SecureStoreUnsupportedSharePointVersionException()
			: base(ErrorCode.rsSecureStoreUnsupportedSharePointVersion, ErrorStrings.rsSecureStoreUnsupportedSharePointVersion, null, null)
		{
		}
	}
}
