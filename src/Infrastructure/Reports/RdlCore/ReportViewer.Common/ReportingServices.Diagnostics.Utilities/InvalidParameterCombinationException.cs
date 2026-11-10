using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidParameterCombinationException : ReportCatalogException
	{
		public InvalidParameterCombinationException()
			: base(ErrorCode.rsInvalidParameterCombination, ErrorStrings.rsInvalidParameterCombination, null, null)
		{
		}
	}
}
