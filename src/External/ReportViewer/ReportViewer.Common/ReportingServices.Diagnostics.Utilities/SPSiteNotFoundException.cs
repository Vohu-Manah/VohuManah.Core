using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class SPSiteNotFoundException : ReportCatalogException
	{
		public SPSiteNotFoundException(string siteId)
			: base(ErrorCode.rsSPSiteNotFound, ErrorStrings.rsSPSiteNotFound(siteId), null, null)
		{
		}
	}
}
