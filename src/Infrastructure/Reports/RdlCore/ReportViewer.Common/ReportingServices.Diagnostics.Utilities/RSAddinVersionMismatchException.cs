using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RSAddinVersionMismatchException : ReportCatalogException
	{
		public RSAddinVersionMismatchException()
			: base(ErrorCode.rsVersionMismatch, ErrorStrings.rsVersionMismatch, null, null)
		{
		}
	}
}
