using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ModelIDMismatchException : ReportCatalogException
	{
		public ModelIDMismatchException()
			: base(ErrorCode.rsModelIDMismatch, ErrorStrings.rsModelIDMismatch, null, null)
		{
		}
	}
}
