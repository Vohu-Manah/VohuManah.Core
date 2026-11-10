using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_NonExistingGlobalReference : Exception
	{
		internal ReportProcessingException_NonExistingGlobalReference(string globalName)
			: base(string.Format(CultureInfo.CurrentCulture, RPRes.rsNonExistingGlobalReference(globalName)))
		{
		}
	}
}
