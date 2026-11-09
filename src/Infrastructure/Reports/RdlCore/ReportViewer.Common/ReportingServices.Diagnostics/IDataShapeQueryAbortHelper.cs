using System;

namespace ReportingServices.Diagnostics
{
	internal interface IDataShapeQueryAbortHelper : IDataShapeAbortHelper, IAbortHelper, IDisposable
	{
		IDataShapeAbortHelper CreateDataShapeAbortHelper();
	}
}
