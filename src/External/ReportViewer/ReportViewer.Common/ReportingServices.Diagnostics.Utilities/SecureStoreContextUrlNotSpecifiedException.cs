using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	internal sealed class SecureStoreContextUrlNotSpecifiedException : ReportCatalogException
	{
		public SecureStoreContextUrlNotSpecifiedException()
			: base(ErrorCode.rsSecureStoreContextUrlNotSpecified, ErrorStrings.rsSecureStoreContextUrlNotSpecified, null, null)
		{
		}
	}
}
