using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DocumentMapEndContainer : IPersistable
	{
		[NonSerialized]
		private static readonly DocumentMapEndContainer m_instance = new DocumentMapEndContainer();

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal static DocumentMapEndContainer Instance => m_instance;

		private DocumentMapEndContainer()
		{
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> memberInfoList = new List<MemberInfo>();
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DocumentMapEndContainer, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, memberInfoList);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				_ = writer.CurrentMember.MemberName;
				Global.Tracer.Assert(condition: false);
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				_ = reader.CurrentMember.MemberName;
				Global.Tracer.Assert(condition: false);
			}
		}

		public void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DocumentMapEndContainer;
		}
	}
}
