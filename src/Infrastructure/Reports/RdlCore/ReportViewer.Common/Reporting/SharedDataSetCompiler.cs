using ReportingServices.DataExtensions;
using ReportingServices.Diagnostics;
using ReportingServices.ReportProcessing;

namespace Reporting
{
	internal delegate DataSetPublishingResult SharedDataSetCompiler(DataSetInfo dataSetInfo, ICatalogItemContext dataSetContext);
}
