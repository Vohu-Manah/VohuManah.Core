using System;

namespace ReportingServices.OnDemandReportRendering
{
	internal interface IMapMapper : IDVMappingLayer, IDisposable
	{
		void RenderMap();
	}
}
