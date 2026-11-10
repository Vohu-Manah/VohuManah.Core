using System.Collections.Generic;

namespace ReportingServices.DataProcessing
{
	internal interface IRestartable
	{
		IDataParameter[] StartAt(List<ScopeValueFieldName> scopeValueFieldNameCollection);
	}
}
