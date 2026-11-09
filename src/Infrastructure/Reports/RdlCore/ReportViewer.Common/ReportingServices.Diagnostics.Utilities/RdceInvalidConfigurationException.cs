using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RdceInvalidConfigurationException : RSException
	{
		public RdceInvalidConfigurationException()
			: base(ErrorCode.rsRdceInvalidConfigurationError, ErrorStrings.rsRdceInvalidConfigurationError, null, RSTrace.IsTraceInitialized ? RSTrace.CatalogTrace : null, null)
		{
		}
	}
}
