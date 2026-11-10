using ReportingServices.Common;
using ReportingServices.Diagnostics;
using ReportingServices.Diagnostics.Utilities;
using ReportingServices.OnDemandReportRendering;
using System;

namespace ReportingServices.ReportProcessing
{
	internal sealed class ReportRendererFactory
	{
		private ReportRendererFactory()
		{
		}

		internal static IRenderingExtension GetNewRenderer(string format, IExtensionFactory extFactory)
		{
			try
			{
				return (IRenderingExtension)extFactory.GetNewRendererExtensionClass(format);
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				throw new ReportProcessingException(ErrorCode.rsRenderingExtensionNotFound);
			}
		}
	}
}
