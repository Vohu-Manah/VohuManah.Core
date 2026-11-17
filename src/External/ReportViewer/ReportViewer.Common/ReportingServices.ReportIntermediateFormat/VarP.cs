using ReportingServices.Diagnostics.Utilities;
using ReportingServices.OnDemandProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	internal class VarP : VarBase
	{
		private static Declaration m_declaration = GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType => DataAggregateInfo.AggregateTypes.VarP;

		internal override object Result()
		{
			switch (m_sumOfXType)
			{
			case ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null:
				return null;
			case ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double:
				return ((double)m_currentCount * (double)m_sumOfXSquared - (double)m_sumOfX * (double)m_sumOfX) / (double)(m_currentCount * m_currentCount);
			case ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal:
				return ((decimal)m_currentCount * (decimal)m_sumOfXSquared - (decimal)m_sumOfX * (decimal)m_sumOfX) / (decimal)(m_currentCount * m_currentCount);
			default:
				Global.Tracer.Assert(condition: false);
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			return new VarP();
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VarP;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VarP, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.VarBase, memberInfoList);
			}
			return m_declaration;
		}
	}
}
