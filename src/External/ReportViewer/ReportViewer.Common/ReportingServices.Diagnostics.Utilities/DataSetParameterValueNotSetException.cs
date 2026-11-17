using System;
using System.Runtime.Serialization;

namespace ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class DataSetParameterValueNotSetException : ReportCatalogException
	{
		public DataSetParameterValueNotSetException(string parameterName)
			: base(ErrorCode.rsDataSetParameterValueNotSet, ErrorStrings.rsDataSetParameterValueNotSet(parameterName), null, parameterName)
		{
		}
	}
}
