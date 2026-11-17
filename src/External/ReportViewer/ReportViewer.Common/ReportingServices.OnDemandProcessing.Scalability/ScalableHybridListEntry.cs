using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class ScalableHybridListEntry : IStorable, IPersistable
	{
		internal object Item;

		internal int Next;

		internal int Previous;

		private static readonly Declaration m_declaration = GetDeclaration();

		public int Size => ItemSizes.SizeOf(Item) + 4 + 4;

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Item:
					writer.WriteVariantOrPersistable(Item);
					break;
				case MemberName.NextLeaf:
					writer.Write(Next);
					break;
				case MemberName.PrevLeaf:
					writer.Write(Previous);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Item:
					Item = reader.ReadVariant();
					break;
				case MemberName.NextLeaf:
					Next = reader.ReadInt32();
					break;
				case MemberName.PrevLeaf:
					Previous = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false, "No references to resolve");
		}

		public ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableHybridListEntry;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Item, Token.Object));
				list.Add(new MemberInfo(MemberName.NextLeaf, Token.Int32));
				list.Add(new MemberInfo(MemberName.PrevLeaf, Token.Int32));
				return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableHybridListEntry, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
