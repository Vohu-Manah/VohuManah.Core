using System.Collections;

namespace ReportingServices.DataProcessing
{
	public interface IDataParameterCollection : IEnumerable
	{
		int Add(IDataParameter parameter);
	}
}
