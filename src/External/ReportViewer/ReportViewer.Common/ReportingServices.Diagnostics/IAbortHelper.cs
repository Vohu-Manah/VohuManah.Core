namespace ReportingServices.Diagnostics
{
	internal interface IAbortHelper
	{
		bool Abort(ProcessingStatus status);
	}
}
