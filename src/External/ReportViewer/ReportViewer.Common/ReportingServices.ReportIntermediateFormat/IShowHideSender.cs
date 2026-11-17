using ReportingServices.ReportProcessing;

namespace ReportingServices.ReportIntermediateFormat
{
	internal interface IShowHideSender
	{
		void ProcessSender(ReportingServices.ReportProcessing.ReportProcessing.ProcessingContext context, int uniqueName);
	}
}
