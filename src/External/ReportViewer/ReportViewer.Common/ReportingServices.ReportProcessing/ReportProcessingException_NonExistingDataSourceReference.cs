using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_NonExistingDataSourceReference : Exception
	{
		internal ReportProcessingException_NonExistingDataSourceReference(string dataSourceName)
			: base(string.Format(CultureInfo.CurrentCulture, RPRes.rsNonExistingDataSourceReference(dataSourceName)))
		{
		}
	}
}
