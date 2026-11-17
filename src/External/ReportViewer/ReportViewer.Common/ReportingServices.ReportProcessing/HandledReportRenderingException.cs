using ReportingServices.Diagnostics.Utilities;
using ReportingServices.OnDemandReportRendering;
using System;
using System.Runtime.Serialization;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class HandledReportRenderingException : RSException
	{
		internal HandledReportRenderingException(ReportRenderingException innerException)
			: base(innerException.ErrorCode, innerException.Message, innerException, Global.RenderingTracer, null)
		{
		}

		internal HandledReportRenderingException(ErrorCode errCode, string message)
			: base(errCode, message, null, Global.RenderingTracer, null)
		{
		}

		internal HandledReportRenderingException(ErrorCode errCode, Exception innerException)
			: base(errCode, innerException.Message, innerException, Global.RenderingTracer, null)
		{
		}
	}
}
