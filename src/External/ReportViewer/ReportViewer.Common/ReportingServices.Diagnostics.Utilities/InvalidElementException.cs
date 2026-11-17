using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidElementException : ReportCatalogException
	{
		public InvalidElementException(string elementName, Exception innerException)
			: base(ErrorCode.rsInvalidElement, ErrorStrings.rsInvalidElement(elementName), innerException, null)
		{
		}

		public InvalidElementException(string elementName)
			: this(elementName, null)
		{
		}
	}
}
