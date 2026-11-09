using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.RdlExpressions;
using ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class TextRunsImpl : TextRuns
	{
		private ReportingServices.ReportIntermediateFormat.TextBox m_textBoxDef;

		private ReportingServices.ReportIntermediateFormat.Paragraph m_paragraphDef;

		private TextRunImpl[] m_textRuns;

		private ReportingServices.RdlExpressions.ReportRuntime m_reportRT;

		private IErrorContext m_iErrorContext;

		private IScope m_scope;

		public override TextRun this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				TextRunImpl textRunImpl = m_textRuns[index];
				if (textRunImpl == null)
				{
					ReportingServices.ReportIntermediateFormat.TextRun textRunDef = m_paragraphDef.TextRuns[index];
					textRunImpl = new TextRunImpl(m_textBoxDef, textRunDef, m_reportRT, m_iErrorContext, m_scope);
					m_textRuns[index] = textRunImpl;
				}
				return textRunImpl;
			}
		}

		internal int Count => m_textRuns.Length;

		internal TextRunsImpl(ReportingServices.ReportIntermediateFormat.Paragraph paragraphDef, ReportingServices.RdlExpressions.ReportRuntime reportRT, IErrorContext iErrorContext, IScope scope)
		{
			m_textBoxDef = paragraphDef.TextBox;
			m_paragraphDef = paragraphDef;
			m_reportRT = reportRT;
			m_iErrorContext = iErrorContext;
			m_scope = scope;
			List<ReportingServices.ReportIntermediateFormat.TextRun> textRuns = m_paragraphDef.TextRuns;
			if (textRuns != null)
			{
				m_textRuns = new TextRunImpl[textRuns.Count];
			}
			else
			{
				m_textRuns = new TextRunImpl[0];
			}
		}

		internal void Reset()
		{
			for (int i = 0; i < m_textRuns.Length; i++)
			{
				m_textRuns[i]?.Reset();
			}
		}
	}
}
