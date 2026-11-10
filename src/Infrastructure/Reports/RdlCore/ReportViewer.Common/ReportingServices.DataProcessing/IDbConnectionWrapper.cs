using System.Data;

namespace ReportingServices.DataProcessing
{
	public interface IDbConnectionWrapper
	{
		System.Data.IDbConnection Connection
		{
			get;
		}
	}
}
