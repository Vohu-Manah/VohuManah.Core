using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.RdlExpressions;
using ReportingServices.ReportIntermediateFormat;

namespace ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class ParagraphImpl : Paragraph
	{
		private TextRunsImpl m_textRuns;

		public override TextRuns TextRuns => m_textRuns;

		internal ParagraphImpl(ReportingServices.ReportIntermediateFormat.Paragraph paragraphDef, ReportingServices.RdlExpressions.ReportRuntime reportRT, IErrorContext iErrorContext, IScope scope)
		{
			m_textRuns = new TextRunsImpl(paragraphDef, reportRT, iErrorContext, scope);
		}

		internal void Reset()
		{
			m_textRuns.Reset();
		}
	}
}
