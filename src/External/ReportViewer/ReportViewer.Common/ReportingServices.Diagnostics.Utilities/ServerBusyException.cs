using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class ServerBusyException : ReportCatalogException
	{
		public ServerBusyException()
			: base(ErrorCode.rsServerBusy, ErrorStrings.rsServerBusy, null, null)
		{
		}
	}
}
