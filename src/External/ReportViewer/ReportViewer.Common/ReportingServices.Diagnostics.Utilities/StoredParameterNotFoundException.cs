using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class StoredParameterNotFoundException : ReportCatalogException
	{
		public StoredParameterNotFoundException(string storedParameterId)
			: base(ErrorCode.rsStoredParameterNotFound, ErrorStrings.rsStoredParameterNotFound(storedParameterId), null, null)
		{
		}
	}
}
