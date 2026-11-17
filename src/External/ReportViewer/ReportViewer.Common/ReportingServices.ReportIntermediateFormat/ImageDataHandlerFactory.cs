using ReportingServices.OnDemandReportRendering;
using ReportingServices.ReportProcessing;
using System;

namespace ReportingServices.ReportIntermediateFormat
{
	internal static class ImageDataHandlerFactory
	{
		public static ImageDataHandler Create(ReportElement reportElement, IBaseImage image)
		{
			switch (image.Source)
			{
			case ReportingServices.OnDemandReportRendering.Image.SourceType.Database:
				return new DatabaseImageDataHandler(reportElement, image);
			case ReportingServices.OnDemandReportRendering.Image.SourceType.Embedded:
				return new EmbeddedImageDataHandler(reportElement, image);
			case ReportingServices.OnDemandReportRendering.Image.SourceType.External:
				return new ExternalImageDataHandler(reportElement, image);
			default:
				Global.Tracer.Assert(false, "Invalid Image.SourceType: {0}", image.Source);
				throw new InvalidOperationException("Invalid Image.SourceType");
			}
		}
	}
}
