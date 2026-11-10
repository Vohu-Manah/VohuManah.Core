using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportProcessing;

namespace ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportVariantProperty : ReportProperty
	{
		private object m_value;

		public object Value => m_value;

		internal ReportVariantProperty()
		{
			m_value = null;
		}

		internal ReportVariantProperty(bool isExpression)
			: base(isExpression, null)
		{
			m_value = null;
		}

		internal ReportVariantProperty(ReportingServices.ReportProcessing.ExpressionInfo expression)
			: base(expression?.IsExpression ?? false, expression?.OriginalText)
		{
			if (expression != null && !expression.IsExpression)
			{
				m_value = expression.Value;
			}
		}

		internal ReportVariantProperty(ReportingServices.ReportIntermediateFormat.ExpressionInfo expression)
			: base(expression?.IsExpression ?? false, expression?.OriginalText)
		{
			if (expression != null && !expression.IsExpression)
			{
				m_value = expression.Value;
			}
		}
	}
}
