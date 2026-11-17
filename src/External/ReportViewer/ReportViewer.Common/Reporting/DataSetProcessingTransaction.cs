using ReportingServices.DataProcessing;
using System;

namespace Reporting
{
	internal class DataSetProcessingTransaction : IDbTransaction, IDisposable
	{
		public void Commit()
		{
		}

		public void Rollback()
		{
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}
