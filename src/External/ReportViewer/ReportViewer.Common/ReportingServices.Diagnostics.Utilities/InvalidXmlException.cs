using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidXmlException : ReportCatalogException
	{
		public InvalidXmlException()
			: base(ErrorCode.rsInvalidXml, ErrorStrings.rsInvalidXml, null, null)
		{
		}
	}
}
