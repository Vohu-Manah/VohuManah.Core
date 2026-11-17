using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SecureConnectionRequiredException : ReportCatalogException
	{
		public SecureConnectionRequiredException()
			: base(ErrorCode.rsSecureConnectionRequired, ErrorStrings.rsSecureConnectionRequired, null, null)
		{
		}
	}
}
