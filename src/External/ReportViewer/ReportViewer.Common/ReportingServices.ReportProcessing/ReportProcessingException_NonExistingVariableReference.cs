using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportProcessingException_NonExistingVariableReference : Exception
	{
		internal ReportProcessingException_NonExistingVariableReference(string varName)
			: base(string.Format(CultureInfo.CurrentCulture, RPRes.rsNonExistingVariableReference(varName)))
		{
		}
	}
}
