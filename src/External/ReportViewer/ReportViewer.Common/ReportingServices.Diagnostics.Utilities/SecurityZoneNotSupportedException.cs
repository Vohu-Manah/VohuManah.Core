using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SecurityZoneNotSupportedException : ReportCatalogException
	{
		public SecurityZoneNotSupportedException()
			: base(ErrorCode.rsSecurityZoneNotSupported, ErrorStrings.rsSecurityZoneNotSupported, null, null)
		{
		}
	}
}
