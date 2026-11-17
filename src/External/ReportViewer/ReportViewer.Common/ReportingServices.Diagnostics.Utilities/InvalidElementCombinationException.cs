using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidElementCombinationException : ReportCatalogException
	{
		public InvalidElementCombinationException(string elementName1, string elementName2)
			: base(ErrorCode.rsInvalidElementCombination, ErrorStrings.rsInvalidElementCombination(elementName1, elementName2), null, null)
		{
		}
	}
}
