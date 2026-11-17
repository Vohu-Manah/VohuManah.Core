using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class OnPremConnectionBuilderUnknownErrorException : ReportCatalogException
	{
		public OnPremConnectionBuilderUnknownErrorException(string connectionString, Exception innerException)
			: base(ErrorCode.rsOnPremConnectionBuilderUnknownError, ErrorStrings.rsOnPremConnectionBuilderUnknownError, innerException, null)
		{
		}
	}
}
