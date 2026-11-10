using ReportingServices.Diagnostics.Utilities;
using ReportingServices.OnDemandProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	internal sealed class StDev : Var
	{
		private static Declaration m_declaration = GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType => DataAggregateInfo.AggregateTypes.StDev;

		internal override object Result()
		{
			if (1 == m_currentCount)
			{
				return null;
			}
			switch (m_sumOfXType)
			{
			case ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null:
				return null;
			case ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double:
				return Math.Sqrt((double)base.Result());
			case ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal:
				return Math.Sqrt(Convert.ToDouble((decimal)base.Result()));
			default:
				Global.Tracer.Assert(condition: false);
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			return new StDev();
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StDev;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StDev, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Var, memberInfoList);
			}
			return m_declaration;
		}
	}
}
