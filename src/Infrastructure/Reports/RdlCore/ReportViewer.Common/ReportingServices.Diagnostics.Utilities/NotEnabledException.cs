using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class NotEnabledException : ReportCatalogException
	{
		public NotEnabledException()
			: base(ErrorCode.rsNotEnabled, ErrorStrings.rsNotEnabled, null, null)
		{
		}
	}
}
