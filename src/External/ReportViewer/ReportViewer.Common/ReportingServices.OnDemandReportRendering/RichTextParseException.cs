using System;
using System.Runtime.Serialization;

namespace ReportingServices.OnDemandReportRendering
{
	[Serializable]
	internal class RichTextParseException : Exception
	{
		internal RichTextParseException(string message)
			: base(message)
		{
		}
	}
}
