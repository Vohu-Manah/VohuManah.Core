using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.RdlExpressions;
using ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class ParagraphsImpl : Paragraphs
	{
		private ReportingServices.ReportIntermediateFormat.TextBox m_textBoxDef;

		private ParagraphImpl[] m_paragraphs;

		private ReportingServices.RdlExpressions.ReportRuntime m_reportRT;

		private IErrorContext m_iErrorContext;

		private IScope m_scope;

		public override Paragraph this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				ParagraphImpl paragraphImpl = m_paragraphs[index];
				if (paragraphImpl == null)
				{
					paragraphImpl = new ParagraphImpl(m_textBoxDef.Paragraphs[index], m_reportRT, m_iErrorContext, m_scope);
					m_paragraphs[index] = paragraphImpl;
				}
				return paragraphImpl;
			}
		}

		internal int Count => m_paragraphs.Length;

		internal ParagraphsImpl(ReportingServices.ReportIntermediateFormat.TextBox textBoxDef, ReportingServices.RdlExpressions.ReportRuntime reportRT, IErrorContext iErrorContext, IScope scope)
		{
			m_textBoxDef = textBoxDef;
			m_reportRT = reportRT;
			m_iErrorContext = iErrorContext;
			m_scope = scope;
			List<ReportingServices.ReportIntermediateFormat.Paragraph> paragraphs = m_textBoxDef.Paragraphs;
			if (paragraphs != null)
			{
				m_paragraphs = new ParagraphImpl[paragraphs.Count];
			}
			else
			{
				m_paragraphs = new ParagraphImpl[0];
			}
		}

		internal void Reset()
		{
			for (int i = 0; i < m_paragraphs.Length; i++)
			{
				m_paragraphs[i]?.Reset();
			}
		}
	}
}
