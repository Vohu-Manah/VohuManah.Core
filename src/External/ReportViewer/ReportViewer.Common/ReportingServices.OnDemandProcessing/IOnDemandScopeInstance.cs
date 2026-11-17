using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.OnDemandProcessing
{
	internal interface IOnDemandScopeInstance : IStorable, IPersistable
	{
		bool IsNoRows
		{
			get;
		}

		bool IsMostRecentlyCreatedScopeInstance
		{
			get;
		}

		bool HasUnProcessedServerAggregate
		{
			get;
		}

		void SetupEnvironment();

		IOnDemandMemberOwnerInstanceReference GetDataRegionInstance(DataRegion rifDataRegion);

		IReference<IDataCorrelation> GetIdcReceiver(IRIFReportDataScope scope);
	}
}
