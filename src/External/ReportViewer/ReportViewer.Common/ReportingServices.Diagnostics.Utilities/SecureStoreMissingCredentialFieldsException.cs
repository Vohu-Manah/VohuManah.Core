using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	internal sealed class SecureStoreMissingCredentialFieldsException : ReportCatalogException
	{
		public SecureStoreMissingCredentialFieldsException(string appId)
			: base(ErrorCode.rsSecureStoreMissingCredentialFields, ErrorStrings.rsSecureStoreMissingCredentialFields(appId), null, null)
		{
		}
	}
}
