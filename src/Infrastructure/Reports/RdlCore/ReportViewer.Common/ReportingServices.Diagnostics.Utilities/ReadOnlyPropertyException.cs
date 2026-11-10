using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ReadOnlyPropertyException : ReportCatalogException
	{
		public ReadOnlyPropertyException(string propertyName)
			: base(ErrorCode.rsReadOnlyProperty, ErrorStrings.rsReadOnlyProperty(propertyName), null, null)
		{
		}
	}
}
