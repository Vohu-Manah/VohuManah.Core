using System;

namespace ReportingServices.DataProcessing
{
	public interface IDbTransactionExtension : IDbTransaction, IDisposable
	{
		bool AllowMultiConnection
		{
			get;
		}
	}
}
