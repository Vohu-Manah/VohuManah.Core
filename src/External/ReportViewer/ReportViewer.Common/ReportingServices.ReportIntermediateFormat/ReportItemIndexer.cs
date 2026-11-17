using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal struct ReportItemIndexer : IPersistable
	{
		internal bool IsComputed;

		internal int Index;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		public object PublishClone(AutomaticSubtotalContext context)
		{
			return MemberwiseClone();
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.IsComputed, Token.Boolean));
			list.Add(new MemberInfo(MemberName.Index, Token.Int32));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemIndexer, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.IsComputed:
					writer.Write(IsComputed);
					break;
				case MemberName.Index:
					writer.Write(Index);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.IsComputed:
					IsComputed = reader.ReadBoolean();
					break;
				case MemberName.Index:
					Index = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItemIndexer;
		}
	}
}
