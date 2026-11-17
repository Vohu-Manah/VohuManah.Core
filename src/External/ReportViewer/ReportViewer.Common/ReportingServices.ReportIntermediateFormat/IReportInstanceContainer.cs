using ReportingServices.OnDemandProcessing;
using ReportingServices.OnDemandProcessing.Scalability;

namespace ReportingServices.ReportIntermediateFormat
{
	internal interface IReportInstanceContainer
	{
		IReference<ReportInstance> ReportInstance
		{
			get;
		}

		IReference<ReportInstance> SetReportInstance(ReportInstance reportInstance, OnDemandMetadata odpMetadata);
	}
}
