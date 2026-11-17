using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ClosingRegisteredStreamException : ReportCatalogException
	{
		public ClosingRegisteredStreamException(Exception innerException)
			: base(ErrorCode.rsClosingRegisteredStreamException, ErrorStrings.rsClosingRegisteredStreamException, innerException, null)
		{
		}
	}
}
