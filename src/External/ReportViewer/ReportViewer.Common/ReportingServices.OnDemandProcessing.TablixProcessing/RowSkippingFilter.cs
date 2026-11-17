using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportProcessing;
using ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;
using System.Diagnostics;

namespace ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class RowSkippingFilter
	{
		private readonly List<ReportingServices.ReportIntermediateFormat.ExpressionInfo> m_expressions;

		private readonly List<object> m_values;

		private readonly OnDemandProcessingContext m_odpContext;

		private readonly IRIFReportDataScope m_scope;

		public RowSkippingFilter(OnDemandProcessingContext odpContext, IRIFReportDataScope scope, List<ReportingServices.ReportIntermediateFormat.ExpressionInfo> expressions, List<object> values)
		{
			m_odpContext = odpContext;
			m_scope = scope;
			m_expressions = expressions;
			m_values = values;
		}

		[Conditional("DEBUG")]
		private void ValidateExpressionsAndValues(List<ReportingServices.ReportIntermediateFormat.ExpressionInfo> expressions, List<object> values)
		{
			Global.Tracer.Assert(expressions != null && values != null && expressions.Count == values.Count, "Invalid expressions or values");
			foreach (ReportingServices.ReportIntermediateFormat.ExpressionInfo expression in expressions)
			{
				Global.Tracer.Assert(expression.Type == ReportingServices.ReportIntermediateFormat.ExpressionInfo.Types.Field, "Only simple field reference expressions can be row skipping filters.");
			}
		}

		public bool ShouldSkipCurrentRow()
		{
			FieldsImpl fieldsImpl = m_odpContext.ReportObjectModel.FieldsImpl;
			bool flag = true;
			for (int i = 0; i < m_expressions.Count && flag; i++)
			{
				ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = m_expressions[i];
				if (expressionInfo.FieldIndex < 0)
				{
					flag = false;
					continue;
				}
				FieldImpl fieldImpl = fieldsImpl[expressionInfo.FieldIndex];
				if (fieldImpl.FieldStatus != 0)
				{
					return false;
				}
				flag = (m_odpContext.CompareAndStopOnError(m_values[i], fieldImpl.Value, m_scope.DataScopeObjectType, m_scope.Name, "GroupExpression", extendedTypeComparisons: false) == 0);
			}
			return flag;
		}
	}
}
