using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	internal sealed class SecureStoreAmbiguousCredentialFieldsException : ReportCatalogException
	{
		public SecureStoreAmbiguousCredentialFieldsException(string appId)
			: base(ErrorCode.rsSecureStoreAmbiguousCredentialFields, ErrorStrings.rsSecureStoreAmbiguousCredentialFields(appId), null, null)
		{
		}
	}
}
