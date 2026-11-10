using ReportingServices.Diagnostics.Utilities;
using System;
using System.Runtime.Serialization;

namespace Reporting
{
	[Serializable]
	internal sealed class DefinitionInvalidException : RSException
	{
		public DefinitionInvalidException(string reportPath, Exception inner)
			: base(ErrorCode.pvInvalidDefinition, ProcessingStrings.pvInvalidDefinition(reportPath), inner, null, null)
		{
		}
	}
}
