using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class MissingSessionIdException : ReportCatalogException
	{
		public MissingSessionIdException()
			: base(ErrorCode.rsMissingSessionId, ErrorStrings.rsMissingSessionId, null, null)
		{
		}
	}
}
