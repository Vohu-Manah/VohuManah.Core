using ReportingServices.Diagnostics.Utilities;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataSetExecutionException : RSException
	{
		internal DataSetExecutionException(ErrorCode code)
			: base(code, RPRes.Keys.GetString(code.ToString()), null, Global.Tracer, null)
		{
		}

		internal DataSetExecutionException(string dataSetName, Exception innerException)
			: base(ErrorCode.rsDataSetExecutionError, string.Format(CultureInfo.CurrentCulture, ErrorStrings.rsDataSetExecutionError(dataSetName)), innerException, Global.Tracer, null)
		{
		}
	}
}
