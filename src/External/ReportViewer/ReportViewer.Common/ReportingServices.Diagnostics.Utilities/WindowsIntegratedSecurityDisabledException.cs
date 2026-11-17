using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class WindowsIntegratedSecurityDisabledException : ReportCatalogException
	{
		public WindowsIntegratedSecurityDisabledException()
			: base(ErrorCode.rsWindowsIntegratedSecurityDisabled, ErrorStrings.rsWindowsIntegratedSecurityDisabled, null, null)
		{
		}
	}
}
