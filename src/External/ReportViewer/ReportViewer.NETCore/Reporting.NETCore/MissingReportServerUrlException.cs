using System;
using System.Runtime.Serialization;

namespace Reporting.NETCore
{
	[Serializable]
	public sealed class MissingReportServerUrlException : ReportViewerException
	{
		public MissingReportServerUrlException()
			: base(CommonStrings.MissingReportServerUrl)
		{
		}
	}
}
