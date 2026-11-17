using System;
using System.Runtime.Serialization;

namespace Reporting.NETCore
{
	[Serializable]
	public sealed class MissingReportSourceException : ReportViewerException
	{
		public MissingReportSourceException()
			: base(CommonStrings.MissingReportSource)
		{
		}
	}
}
