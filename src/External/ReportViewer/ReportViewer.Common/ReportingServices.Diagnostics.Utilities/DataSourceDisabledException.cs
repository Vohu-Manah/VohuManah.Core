using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class DataSourceDisabledException : ReportCatalogException
	{
		public DataSourceDisabledException()
			: base(ErrorCode.rsDataSourceDisabled, ErrorStrings.rsDataSourceDisabled, null, null)
		{
		}
	}
}
