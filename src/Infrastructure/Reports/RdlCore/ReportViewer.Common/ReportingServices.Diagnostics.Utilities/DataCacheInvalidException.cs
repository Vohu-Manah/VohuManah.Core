using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	internal sealed class DataCacheInvalidException : RSException
	{
		public DataCacheInvalidException(Exception innerException)
			: base(ErrorCode.rsDataCacheMismatch, ErrorStrings.rsDataCacheMismatch, innerException, RSTrace.IsTraceInitialized ? RSTrace.CatalogTrace : null, null)
		{
		}
	}
}
