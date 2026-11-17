using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	internal sealed class SecureStoreUnsupportedCredentialFieldException : ReportCatalogException
	{
		public SecureStoreUnsupportedCredentialFieldException(string appId)
			: base(ErrorCode.rsSecureStoreUnsupportedCredentialField, ErrorStrings.rsSecureStoreUnsupportedCredentialField(appId), null, null)
		{
		}
	}
}
