using ReportingServices.OnDemandProcessing;
using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.OnDemandReportRendering;
using ReportingServices.RdlExpressions;
using ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class TextRunImpl : TextRun
	{
		private ReportingServices.ReportIntermediateFormat.TextBox m_textBoxDef;

		private ReportingServices.ReportIntermediateFormat.TextRun m_textRunDef;

		private ReportingServices.RdlExpressions.ReportRuntime m_reportRT;

		private IErrorContext m_iErrorContext;

		private ReportingServices.RdlExpressions.VariantResult m_result;

		private bool m_isValueReady;

		private bool m_isVisited;

		private IScope m_scope;

		private List<string> m_fieldsUsedInValueExpression;

		public override object Value
		{
			get
			{
				GetResult(null);
				return m_result.Value;
			}
		}

		internal ReportingServices.ReportIntermediateFormat.TextRun TextRunDef => m_textRunDef;

		internal TextRunImpl(ReportingServices.ReportIntermediateFormat.TextBox textBoxDef, ReportingServices.ReportIntermediateFormat.TextRun textRunDef, ReportingServices.RdlExpressions.ReportRuntime reportRT, IErrorContext iErrorContext, IScope scope)
		{
			m_textBoxDef = textBoxDef;
			m_textRunDef = textRunDef;
			m_reportRT = reportRT;
			m_iErrorContext = iErrorContext;
			m_scope = scope;
		}

		internal ReportingServices.RdlExpressions.VariantResult GetResult(IReportScopeInstance romInstance)
		{
			if (!m_isValueReady)
			{
				if (m_isVisited)
				{
					m_iErrorContext.Register(ProcessingErrorCode.rsCyclicExpression, Severity.Warning, m_textRunDef.ObjectType, m_textRunDef.Name, "Value");
					throw new ReportProcessingException_InvalidOperationException();
				}
				m_isVisited = true;
				ObjectType objectType = m_reportRT.ObjectType;
				string objectName = m_reportRT.ObjectName;
				string propertyName = m_reportRT.PropertyName;
				IScope currentScope = m_reportRT.CurrentScope;
				m_reportRT.CurrentScope = m_scope;
				OnDemandProcessingContext odpContext = m_reportRT.ReportObjectModel.OdpContext;
				ObjectModelImpl reportObjectModel = m_reportRT.ReportObjectModel;
				try
				{
					odpContext.SetupContext(m_textBoxDef, romInstance);
					bool num = (m_textRunDef.Action != null && m_textRunDef.Action.TrackFieldsUsedInValueExpression) || (m_textBoxDef != null && m_textBoxDef.Action != null && m_textBoxDef.Action.TrackFieldsUsedInValueExpression);
					if (num)
					{
						reportObjectModel.ResetFieldsUsedInExpression();
					}
					m_result = m_reportRT.EvaluateTextRunValueExpression(m_textRunDef);
					if (num)
					{
						m_fieldsUsedInValueExpression = new List<string>();
						reportObjectModel.AddFieldsUsedInExpression(m_fieldsUsedInValueExpression);
					}
				}
				finally
				{
					m_reportRT.CurrentScope = currentScope;
					m_reportRT.ObjectType = objectType;
					m_reportRT.ObjectName = objectName;
					m_reportRT.PropertyName = propertyName;
					m_isVisited = false;
					m_isValueReady = true;
				}
			}
			return m_result;
		}

		internal List<string> GetFieldsUsedInValueExpression(IReportScopeInstance romInstance)
		{
			if (!m_isValueReady)
			{
				GetResult(romInstance);
			}
			return m_fieldsUsedInValueExpression;
		}

		internal void MergeFieldsUsedInValueExpression(Dictionary<string, bool> usedFields)
		{
			if (m_fieldsUsedInValueExpression == null)
			{
				return;
			}
			for (int i = 0; i < m_fieldsUsedInValueExpression.Count; i++)
			{
				string text = m_fieldsUsedInValueExpression[i];
				if (text != null)
				{
					usedFields[text] = true;
				}
			}
		}

		internal void Reset()
		{
			if (m_isValueReady && m_textRunDef.Value.IsExpression)
			{
				m_isValueReady = false;
			}
		}
	}
}
