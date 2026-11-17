using ReportingServices.ReportProcessing;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class RichTextHelpers
	{
		internal static MarkupType TranslateMarkupType(string value)
		{
			if (ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(value, "None", ignoreCase: false) == 0)
			{
				return MarkupType.None;
			}
			if (ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(value, "HTML", ignoreCase: false) == 0)
			{
				return MarkupType.HTML;
			}
			return MarkupType.None;
		}

		internal static ListStyle TranslateListStyle(string value)
		{
			if (ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(value, "None", ignoreCase: false) == 0)
			{
				return ListStyle.None;
			}
			if (ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(value, "Numbered", ignoreCase: false) == 0)
			{
				return ListStyle.Numbered;
			}
			if (ReportingServices.ReportProcessing.ReportProcessing.CompareWithInvariantCulture(value, "Bulleted", ignoreCase: false) == 0)
			{
				return ListStyle.Bulleted;
			}
			return ListStyle.None;
		}
	}
}
