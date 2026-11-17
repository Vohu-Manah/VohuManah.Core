using ReportingServices.Interfaces;
using System.IO;

namespace ReportingServices.OnDemandReportRendering
{
	internal interface IDataSegmentRenderer
	{
		void RenderSegment(Report report, Stream dataSegmentQuery, CreateAndRegisterStream createAndRegisterStream);

		void ExecuteQueries(Stream executeQueriesRequest, ExecuteQueriesContext executeQueriesContext);
	}
}
