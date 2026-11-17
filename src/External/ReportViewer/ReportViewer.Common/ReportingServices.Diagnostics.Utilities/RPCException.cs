using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class RPCException : ReportCatalogException
	{
		public RPCException(Exception exceptionFromRPC)
			: base(ErrorCode.rsRPCError, exceptionFromRPC.Message, exceptionFromRPC.InnerException, null)
		{
		}
	}
}
