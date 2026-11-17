using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_NonExistingLookupReference : Exception
	{
		internal ReportProcessingException_NonExistingLookupReference()
			: base(string.Format(CultureInfo.CurrentCulture, RPRes.rsNonExistingLookupReference))
		{
		}
	}
}
