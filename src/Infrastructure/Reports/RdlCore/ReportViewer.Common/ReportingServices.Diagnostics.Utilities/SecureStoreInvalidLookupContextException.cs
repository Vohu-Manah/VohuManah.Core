using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	internal sealed class SecureStoreInvalidLookupContextException : ReportCatalogException
	{
		public SecureStoreInvalidLookupContextException()
			: base(ErrorCode.rsSecureStoreInvalidLookupContext, ErrorStrings.rsSecureStoreInvalidLookupContext, null, null)
		{
		}
	}
}
