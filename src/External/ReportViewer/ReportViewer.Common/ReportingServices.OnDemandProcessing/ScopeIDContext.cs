using Cloud.Platform.Utils;
using ReportingServices.Common;
using ReportingServices.DataProcessing;
using ReportingServices.OnDemandReportRendering;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Diagnostics;

namespace ReportingServices.OnDemandProcessing
{
	internal sealed class ScopeIDContext : RestartContext
	{
		private readonly ScopeID m_scopeID;

		private readonly ReportingServices.ReportIntermediateFormat.ReportHierarchyNode m_memberDef;

		private readonly InternalStreamingOdpDynamicMemberLogic m_memberLogic;

		internal ScopeID ScopeID => m_scopeID;

		internal ReportingServices.ReportIntermediateFormat.ReportHierarchyNode MemberDefinition => m_memberDef;

		internal InternalStreamingOdpDynamicMemberLogic MemberLogic => m_memberLogic;

		internal List<ReportingServices.ReportIntermediateFormat.ExpressionInfo> Expressions
		{
			get
			{
				if (m_memberDef.Sorting != null && m_memberDef.Sorting.NaturalSort)
				{
					return m_memberDef.Sorting.SortExpressions;
				}
				return m_memberDef.Grouping.GroupExpressions;
			}
		}

		internal List<bool> SortDirections => m_memberDef.Sorting.SortDirections;

		internal ScopeIDContext(ScopeID scopeID, ReportingServices.ReportIntermediateFormat.ReportHierarchyNode memberDef, InternalStreamingOdpDynamicMemberLogic memberLogic, RestartMode restartMode)
			: base(restartMode)
		{
			m_scopeID = scopeID;
			m_memberDef = memberDef;
			m_memberLogic = memberLogic;
		}

		public bool RomBasedRestart()
		{
			return m_memberLogic.RomBasedRestart(m_scopeID);
		}

		public override List<ScopeValueFieldName> GetScopeValueFieldNameCollection(ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			List<ScopeValueFieldName> list = new List<ScopeValueFieldName>();
			int num = 0;
			foreach (ScopeValue item in m_scopeID.QueryRestartPosition)
			{
				ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = Expressions[num];
				string dataField = dataSet.Fields[expressionInfo.FieldIndex].DataField;
				list.Add(new ScopeValueFieldName(dataField, item.Value));
				num++;
			}
			return list;
		}

		public override RowSkippingControlFlag DoesNotMatchRowRecordField(OnDemandProcessingContext odpContext, ReportingServices.ReportIntermediateFormat.RecordField[] recordFields)
		{
			int num = 0;
			foreach (ScopeValue item in m_scopeID.QueryRestartPosition)
			{
				ReportingServices.ReportIntermediateFormat.ExpressionInfo expressionInfo = Expressions[num];
				ReportingServices.ReportIntermediateFormat.RecordField field = recordFields[expressionInfo.FieldIndex];
				RowSkippingControlFlag rowSkippingControlFlag = CompareFieldWithScopeValueAndStopOnInequality(odpContext, field, item.Value, SortDirections[num], ObjectType.DataSet, m_memberDef.DataScopeInfo.DataSet.Name, "ScopeID.QueryRestart");
				if (rowSkippingControlFlag != 0)
				{
					return rowSkippingControlFlag;
				}
				num++;
			}
			return RowSkippingControlFlag.ExactMatch;
		}

		public override void TraceStartAtRecoveryMessage()
		{
			Global.Tracer.Trace(TraceLevel.Warning, "START AT Recovery Mode: Target row grouping {0} did not match with ScopeID = {1}.", m_memberDef.Grouping.Name.MarkAsModelInfo(), m_scopeID.ToString());
		}
	}
}
