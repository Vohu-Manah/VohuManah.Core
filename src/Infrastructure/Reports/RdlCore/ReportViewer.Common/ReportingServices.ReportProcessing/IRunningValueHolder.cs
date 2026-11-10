namespace ReportingServices.ReportProcessing
{
	internal interface IRunningValueHolder
	{
		RunningValueInfoList GetRunningValueList();

		void ClearIfEmpty();
	}
}
