using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class DataSetNotFoundException : ReportCatalogException
	{
		public DataSetNotFoundException(string dataSet)
			: base(ErrorCode.rsDataSetNotFound, ErrorStrings.rsDataSetNotFound(dataSet), null, null)
		{
		}
	}
}
