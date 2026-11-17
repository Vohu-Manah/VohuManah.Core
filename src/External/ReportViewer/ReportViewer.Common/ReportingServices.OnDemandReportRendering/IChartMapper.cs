using System;

namespace ReportingServices.OnDemandReportRendering
{
	internal interface IChartMapper : IDVMappingLayer, IDisposable
	{
		void RenderChart();
	}
}
