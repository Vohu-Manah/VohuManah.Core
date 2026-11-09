using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class OperationNotSupportedSharePointModeException : ReportCatalogException
	{
		public OperationNotSupportedSharePointModeException()
			: base(ErrorCode.rsOperationNotSupportedSharePointMode, ErrorStrings.rsOperationNotSupportedSharePointMode, null, null)
		{
		}
	}
}
