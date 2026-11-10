using System;
using System.Runtime.Serialization;

namespace Reporting.NETCore
{
	[Serializable]
	public sealed class LocalProcessingException : ReportViewerException
	{
		internal LocalProcessingException(Exception processingException)
			: base(CommonStrings.LocalProcessingErrors, processingException)
		{
		}

		internal LocalProcessingException(string message, Exception processingException)
			: base(message, processingException)
		{
		}

		internal LocalProcessingException(string message)
			: base(message)
		{
		}
	}
}
