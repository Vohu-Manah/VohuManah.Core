using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class DataSourceNotFoundException : ReportCatalogException
	{
		public DataSourceNotFoundException(string dataSource)
			: base(ErrorCode.rsDataSourceNotFound, ErrorStrings.rsDataSourceNotFound(dataSource), null, null)
		{
		}
	}
}
