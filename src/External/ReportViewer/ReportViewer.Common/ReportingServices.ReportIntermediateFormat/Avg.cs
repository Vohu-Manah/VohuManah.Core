using ReportingServices.Diagnostics.Utilities;
using ReportingServices.OnDemandProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	internal sealed class Avg : Sum
	{
		private uint m_currentCount;

		private static Declaration m_declaration = GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType => DataAggregateInfo.AggregateTypes.Avg;

		public override int Size => base.Size + 4;

		internal override void Init()
		{
			base.Init();
			m_currentCount = 0u;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			if (!DataAggregate.IsNull(DataAggregate.GetTypeCode(expressions[0])))
			{
				base.Update(expressions, iErrorContext);
				m_currentCount++;
			}
		}

		internal override object Result()
		{
			switch (m_currentTotalType)
			{
			case ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Null:
				return null;
			case ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Double:
				return (double)m_currentTotal / (double)m_currentCount;
			case ReportingServices.ReportProcessing.DataAggregate.DataTypeCode.Decimal:
				return (decimal)m_currentTotal / (decimal)m_currentCount;
			default:
				Global.Tracer.Assert(condition: false);
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			return new Avg();
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.CurrentCount)
				{
					writer.Write(m_currentCount);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.CurrentCount)
				{
					m_currentCount = reader.ReadUInt32();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false, "Avg should not resolve references");
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Avg;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.CurrentCount, Token.UInt32));
				return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Avg, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Sum, list);
			}
			return m_declaration;
		}
	}
}
