using ReportingServices.DataProcessing;
using ReportingServices.Diagnostics.Utilities;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing
{
	internal abstract class RestartContext
	{
		private readonly RestartMode m_restartMode;

		public RestartMode RestartMode => m_restartMode;

		public bool IsRowLevelRestart => m_restartMode != RestartMode.Rom;

		public RestartContext(RestartMode mode)
		{
			m_restartMode = mode;
		}

		public abstract RowSkippingControlFlag DoesNotMatchRowRecordField(OnDemandProcessingContext odpContext, ReportingServices.ReportIntermediateFormat.RecordField[] recordFields);

		public RowSkippingControlFlag CompareFieldWithScopeValueAndStopOnInequality(OnDemandProcessingContext odpContext, ReportingServices.ReportIntermediateFormat.RecordField field, object scopeValue, bool isSortedAscending, ObjectType objectType, string objectName, string propertyName)
		{
			if (field == null)
			{
				throw new ReportProcessingException(ErrorCode.rsMissingFieldInStartAt);
			}
			int num = odpContext.CompareAndStopOnError(field.FieldValue, scopeValue, objectType, objectName, propertyName, extendedTypeComparisons: false);
			if (num < 0)
			{
				if (!isSortedAscending)
				{
					return RowSkippingControlFlag.Stop;
				}
				return RowSkippingControlFlag.Skip;
			}
			if (num > 0)
			{
				if (!isSortedAscending)
				{
					return RowSkippingControlFlag.Skip;
				}
				return RowSkippingControlFlag.Stop;
			}
			return RowSkippingControlFlag.ExactMatch;
		}

		public abstract List<ScopeValueFieldName> GetScopeValueFieldNameCollection(ReportingServices.ReportIntermediateFormat.DataSet dataSet);

		public abstract void TraceStartAtRecoveryMessage();
	}
}
