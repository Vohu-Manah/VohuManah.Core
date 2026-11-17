using ReportingServices.OnDemandProcessing;
using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	internal sealed class First : DataAggregate
	{
		private object m_value;

		private bool m_updated;

		private static Declaration m_declaration = GetDeclaration();

		internal override DataAggregateInfo.AggregateTypes AggregateType => DataAggregateInfo.AggregateTypes.First;

		public override int Size => ItemSizes.SizeOf(m_value) + 1;

		internal override void Init()
		{
			m_value = null;
			m_updated = false;
		}

		internal override void Update(object[] expressions, IErrorContext iErrorContext)
		{
			if (!m_updated)
			{
				m_value = expressions[0];
				m_updated = true;
			}
		}

		internal override object Result()
		{
			return m_value;
		}

		internal override DataAggregate ConstructAggregator(OnDemandProcessingContext odpContext, DataAggregateInfo aggregateDef)
		{
			return new First();
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Value:
					writer.Write(m_value);
					break;
				case MemberName.Updated:
					writer.Write(m_updated);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Value:
					m_value = reader.ReadVariant();
					break;
				case MemberName.Updated:
					m_updated = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false, "First should not resolve references");
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.First;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Value, Token.Object));
				list.Add(new MemberInfo(MemberName.Updated, Token.Boolean));
				return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.First, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregate, list);
			}
			return m_declaration;
		}
	}
}
