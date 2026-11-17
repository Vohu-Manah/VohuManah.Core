using ReportingServices.ReportIntermediateFormat;
using System;

namespace ReportingServices.OnDemandProcessing
{
	internal interface IRecordRowReader : IDisposable
	{
		RecordRow RecordRow
		{
			get;
		}

		bool GetNextRow();

		bool MoveToFirstRow();

		void Close();
	}
}
