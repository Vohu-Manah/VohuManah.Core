using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal abstract class ReportCatalogException : RSException
	{
		public ReportCatalogException(ErrorCode errorCode, string localizedMessage, Exception innerException, string additionalTraceMessage, params object[] exceptionData)
			: base(errorCode, localizedMessage, innerException, RSTrace.IsTraceInitialized ? RSTrace.CatalogTrace : null, additionalTraceMessage, exceptionData)
		{
		}
	}
}
