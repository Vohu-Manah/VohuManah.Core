using ReportingServices.DataProcessing;
using ReportingServices.RdlExpressions;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing
{
	internal sealed class RelationshipRestartContext : RestartContext
	{
		private ReportingServices.ReportIntermediateFormat.ExpressionInfo[] m_expressions;

		private ReportingServices.RdlExpressions.VariantResult[] m_values;

		private readonly ReportingServices.ReportIntermediateFormat.DataSet m_idcDataSet;

		private SortDirection[] m_sortDirections;

		public RelationshipRestartContext(ReportingServices.ReportIntermediateFormat.ExpressionInfo[] expressions, ReportingServices.RdlExpressions.VariantResult[] values, SortDirection[] sortDirections, ReportingServices.ReportIntermediateFormat.DataSet idcDataSet)
			: base(RestartMode.Query)
		{
			m_expressions = expressions;
			m_values = values;
			m_idcDataSet = idcDataSet;
			m_sortDirections = sortDirections;
			NormalizeValues(m_values);
		}

		public override RowSkippingControlFlag DoesNotMatchRowRecordField(OnDemandProcessingContext odpContext, ReportingServices.ReportIntermediateFormat.RecordField[] recordFields)
		{
			for (int i = 0; i < m_expressions.Length; i++)
			{
				ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = m_expressions[i];
				object value = m_values[i].Value;
				ReportingServices.ReportIntermediateFormat.RecordField field = recordFields[expressionInfo.FieldIndex];
				bool isSortedAscending = m_sortDirections[i] == SortDirection.Ascending;
				RowSkippingControlFlag rowSkippingControlFlag = CompareFieldWithScopeValueAndStopOnInequality(odpContext, field, value, isSortedAscending, ObjectType.DataSet, m_idcDataSet.Name, "Relationship.QueryRestart");
				if (rowSkippingControlFlag != 0)
				{
					return rowSkippingControlFlag;
				}
			}
			return RowSkippingControlFlag.ExactMatch;
		}

		private void NormalizeValues(ReportingServices.RdlExpressions.VariantResult[] values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				if (values[i].Value is DBNull)
				{
					values[i].Value = null;
				}
			}
		}

		public override List<ScopeValueFieldName> GetScopeValueFieldNameCollection(ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			List<ScopeValueFieldName> list = new List<ScopeValueFieldName>();
			for (int i = 0; i < m_expressions.Length; i++)
			{
				string dataField = dataSet.Fields[m_expressions[i].FieldIndex].DataField;
				list.Add(new ScopeValueFieldName(dataField, m_values[i].Value));
			}
			return list;
		}

		public override void TraceStartAtRecoveryMessage()
		{
		}
	}
}
