using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ElementTypeMismatchException : ReportCatalogException
	{
		public ElementTypeMismatchException(string elementName)
			: base(ErrorCode.rsElementTypeMismatch, ErrorStrings.rsElementTypeMismatch(elementName), null, null)
		{
		}

		public ElementTypeMismatchException(string elementName, Exception innerException)
			: base(ErrorCode.rsElementTypeMismatch, ErrorStrings.rsElementTypeMismatch(elementName), innerException, null)
		{
		}
	}
}
