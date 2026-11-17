using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.OnDemandProcessing
{
	[PersistedWithinRequestOnly]
	[SkipStaticValidation]
	internal class StreamingNoRowsCellInstance : StreamingNoRowsScopeInstanceBase
	{
		public StreamingNoRowsCellInstance(OnDemandProcessingContext odpContext, IRIFReportDataScope cell)
			: base(odpContext, cell)
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.StreamingNoRowsCellInstance;
		}
	}
}
