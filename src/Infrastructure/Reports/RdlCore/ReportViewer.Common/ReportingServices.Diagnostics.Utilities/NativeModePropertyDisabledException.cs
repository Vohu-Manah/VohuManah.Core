using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class NativeModePropertyDisabledException : ReportCatalogException
	{
		public NativeModePropertyDisabledException()
			: base(ErrorCode.rsPropertyDisabledNativeMode, ErrorStrings.rsPropertyDisabledNativeMode, null, null)
		{
		}
	}
}
