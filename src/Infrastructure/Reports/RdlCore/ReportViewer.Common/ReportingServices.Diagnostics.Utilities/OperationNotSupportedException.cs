using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class OperationNotSupportedException : ReportCatalogException
	{
		public OperationNotSupportedException(string operation)
			: base(ErrorCode.rsOperationNotSupported, ErrorStrings.rsOperationNotSupported(operation), null, null)
		{
		}
	}
}
