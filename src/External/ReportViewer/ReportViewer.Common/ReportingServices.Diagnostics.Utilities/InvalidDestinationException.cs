using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidDestinationException : ReportCatalogException
	{
		public InvalidDestinationException(string sourcePath, string targetPath)
			: base(ErrorCode.rsInvalidDestination, ErrorStrings.rsInvalidDestination(sourcePath, targetPath), null, null)
		{
		}
	}
}
