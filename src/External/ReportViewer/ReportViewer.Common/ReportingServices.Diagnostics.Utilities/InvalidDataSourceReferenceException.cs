using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class InvalidDataSourceReferenceException : ReportCatalogException
	{
		public InvalidDataSourceReferenceException(string datasourceName)
			: base(ErrorCode.rsInvalidDataSourceReference, ErrorStrings.rsInvalidDataSourceReference(datasourceName), null, null)
		{
		}
	}
}
