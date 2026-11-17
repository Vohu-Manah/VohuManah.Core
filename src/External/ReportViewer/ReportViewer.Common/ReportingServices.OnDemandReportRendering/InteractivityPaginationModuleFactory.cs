using ReportingServices.Rendering.SPBProcessing;

namespace ReportingServices.OnDemandReportRendering
{
	internal static class InteractivityPaginationModuleFactory
	{
		internal static IInteractivityPaginationModule CreateInteractivityPaginationModule()
		{
			return new SPBInteractivityProcessing();
		}
	}
}
