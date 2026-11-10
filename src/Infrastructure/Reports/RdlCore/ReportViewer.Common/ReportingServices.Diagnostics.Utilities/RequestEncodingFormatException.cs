using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RequestEncodingFormatException : ReportCatalogException
	{
		public RequestEncodingFormatException(Exception innerException)
			: base(ErrorCode.rsRequestEncodingFormatException, ErrorStrings.rsRequestEncodingFormatException, innerException, null)
		{
		}
	}
}
