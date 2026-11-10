using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ModelGenerationErrorException : ReportCatalogException
	{
		public ModelGenerationErrorException(Exception innerException)
			: base(ErrorCode.rsModelGenerationError, ErrorStrings.rsModelGenerationError, innerException, null)
		{
		}
	}
}
