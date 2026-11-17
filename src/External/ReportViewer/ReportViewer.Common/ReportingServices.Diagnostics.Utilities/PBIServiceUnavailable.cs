using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class PBIServiceUnavailable : ReportCatalogException
	{
		public PBIServiceUnavailable(string correlationId)
			: base(ErrorCode.rsPBIServiceUnavailable, ErrorStrings.rsPBIServiceUnavailable(correlationId), null, null)
		{
		}
	}
}
