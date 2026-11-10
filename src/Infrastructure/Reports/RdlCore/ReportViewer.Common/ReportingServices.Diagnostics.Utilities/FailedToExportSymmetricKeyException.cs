using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class FailedToExportSymmetricKeyException : ReportCatalogException
	{
		public FailedToExportSymmetricKeyException()
			: base(ErrorCode.rsFailedToExportSymmetricKey, ErrorStrings.rsFailedToExportSymmetricKey, null, null)
		{
		}
	}
}
