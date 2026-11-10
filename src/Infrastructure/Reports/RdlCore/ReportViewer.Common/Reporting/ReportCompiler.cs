using ReportingServices.Diagnostics;
using ReportingServices.Library;
using ReportingServices.ReportProcessing;
using System;

namespace Reporting
{
	internal static class ReportCompiler
	{
		public static PublishingResult CompileReport(ICatalogItemContext context, byte[] reportDefinition, bool generateExpressionHostWithRefusedPermissions, out ControlSnapshot snapshot)
		{
			snapshot = null;
			try
			{
				ReportProcessing reportProcessing = new ReportProcessing();
				snapshot = new ControlSnapshot();
				PublishingContext reportPublishingContext = new PublishingContext(context, reportDefinition, snapshot, null, generateExpressionHostWithRefusedPermissions, snapshot.ReportProcessingFlags, reportProcessing.Configuration, DataProtectionLocal.Instance);
				return reportProcessing.CreateIntermediateFormat(reportPublishingContext);
			}
			catch (Exception inner)
			{
				string text = context.ItemPathAsString;
				if (text == null)
				{
					text = ProcessingStrings.MainReport;
				}
				throw new DefinitionInvalidException(text, inner);
			}
		}
	}
}
