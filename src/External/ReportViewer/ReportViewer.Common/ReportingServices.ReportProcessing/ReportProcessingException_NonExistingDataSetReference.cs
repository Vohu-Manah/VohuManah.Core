using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_NonExistingDataSetReference : Exception
	{
		internal ReportProcessingException_NonExistingDataSetReference(string dataSetName)
			: base(string.Format(CultureInfo.CurrentCulture, RPRes.rsNonExistingDataSetReference(dataSetName)))
		{
		}
	}
}
