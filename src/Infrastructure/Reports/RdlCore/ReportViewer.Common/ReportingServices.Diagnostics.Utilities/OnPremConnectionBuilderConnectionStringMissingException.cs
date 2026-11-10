using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class OnPremConnectionBuilderConnectionStringMissingException : ReportCatalogException
	{
		public OnPremConnectionBuilderConnectionStringMissingException()
			: base(ErrorCode.rsOnPremConnectionBuilderConnectionStringMissing, ErrorStrings.rsOnPremConnectionBuilderConnectionStringMissing, null, null)
		{
		}
	}
}
