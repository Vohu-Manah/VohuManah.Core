using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class CacheRefreshPlanNotFoundException : ReportCatalogException
	{
		public CacheRefreshPlanNotFoundException(string idOrData)
			: base(ErrorCode.rsCacheRefreshPlanNotFound, ErrorStrings.rsCacheRefreshPlanNotFound(idOrData), null, null)
		{
		}
	}
}
