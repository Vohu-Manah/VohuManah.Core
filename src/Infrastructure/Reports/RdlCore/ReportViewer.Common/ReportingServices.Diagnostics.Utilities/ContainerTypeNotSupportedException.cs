using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ContainerTypeNotSupportedException : ReportCatalogException
	{
		public ContainerTypeNotSupportedException()
			: base(ErrorCode.rsContainerNotSupported, ErrorStrings.rsContainerNotSupported, null, null)
		{
		}
	}
}
