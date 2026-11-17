using System;

namespace ReportingServices.DataProcessing
{
	public interface IDbTransaction : IDisposable
	{
		void Commit();

		void Rollback();
	}
}
