using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal interface ISortDataHolder : IStorable, IPersistable
	{
		void NextRow();

		void Traverse(ProcessingStages operation, ITraversalContext traversalContext);
	}
}
