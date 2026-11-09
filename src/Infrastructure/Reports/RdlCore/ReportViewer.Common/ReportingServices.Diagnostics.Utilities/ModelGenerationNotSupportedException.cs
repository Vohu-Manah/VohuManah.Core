using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ModelGenerationNotSupportedException : ReportCatalogException
	{
		public ModelGenerationNotSupportedException()
			: base(ErrorCode.rsModelGenerationNotSupported, ErrorStrings.rsModelGenerationNotSupported, null, null)
		{
		}
	}
}
