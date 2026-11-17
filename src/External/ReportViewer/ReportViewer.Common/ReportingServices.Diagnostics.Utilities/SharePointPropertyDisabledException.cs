using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SharePointPropertyDisabledException : ReportCatalogException
	{
		public SharePointPropertyDisabledException()
			: base(ErrorCode.rsPropertyDisabled, ErrorStrings.rsPropertyDisabled, null, null)
		{
		}
	}
}
