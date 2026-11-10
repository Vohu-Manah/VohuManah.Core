using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class DataSourceOpenException : ReportCatalogException
	{
		public DataSourceOpenException(string datasourceName, Exception innerException)
			: base(ErrorCode.rsErrorOpeningConnection, ErrorStrings.rsErrorOpeningConnection(datasourceName), innerException, null)
		{
		}
	}
}
