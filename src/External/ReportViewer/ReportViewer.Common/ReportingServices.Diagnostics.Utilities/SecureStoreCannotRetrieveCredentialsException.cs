using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	internal sealed class SecureStoreCannotRetrieveCredentialsException : ReportCatalogException
	{
		public SecureStoreCannotRetrieveCredentialsException(Exception innerException)
			: base(ErrorCode.rsSecureStoreCannotRetrieveCredentials, ErrorStrings.rsSecureStoreCannotRetrieveCredentials(innerException.Message), innerException, null)
		{
		}
	}
}
