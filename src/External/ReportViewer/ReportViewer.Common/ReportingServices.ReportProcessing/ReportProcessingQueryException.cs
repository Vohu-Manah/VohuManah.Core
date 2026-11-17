using ReportingServices.Diagnostics.Utilities;
using System;
using System.Runtime.Serialization;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingQueryException : ReportProcessingException
	{
		public ReportProcessingQueryException(ErrorCode errorCode, Exception innerException, params object[] arguments)
			: base(errorCode, innerException, arguments)
		{
		}
	}
}
