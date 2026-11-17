using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IStorable : IPersistable
	{
		int Size
		{
			get;
		}
	}
}
