using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	internal interface IRunningValueHolder
	{
		DataScopeInfo DataScopeInfo
		{
			get;
		}

		List<RunningValueInfo> GetRunningValueList();

		void ClearIfEmpty();
	}
}
