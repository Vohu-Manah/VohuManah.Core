using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RdceInvalidCacheOptionException : RSException
	{
		public RdceInvalidCacheOptionException()
			: base(ErrorCode.rsRdceInvalidCacheOptionError, ErrorStrings.rsRdceInvalidCacheOptionError, null, RSTrace.IsTraceInitialized ? RSTrace.CatalogTrace : null, null)
		{
		}
	}
}
