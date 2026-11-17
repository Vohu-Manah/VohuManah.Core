using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RemotePublicKeyUnavailableException : ReportCatalogException
	{
		public RemotePublicKeyUnavailableException()
			: base(ErrorCode.rsRemotePublicKeyUnavailable, ErrorStrings.rsRemotePublicKeyUnavailable, null, null)
		{
		}
	}
}
