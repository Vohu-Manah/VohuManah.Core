using ReportingServices.OnDemandProcessing;
using ReportingServices.ReportIntermediateFormat;

namespace ReportingServices.ReportProcessing
{
	internal interface IRowConsumer
	{
		string ReportDataSetName
		{
			get;
		}

		void SetProcessingDataReader(IProcessingDataReader dataReader);

		void NextRow(ReportingServices.ReportIntermediateFormat.RecordRow row);
	}
}
