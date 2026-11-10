using System;

namespace ReportingServices.OnDemandReportRendering
{
	internal interface IGaugeMapper : IDVMappingLayer, IDisposable
	{
		void RenderGaugePanel();

		void RenderDataGaugePanel();
	}
}
