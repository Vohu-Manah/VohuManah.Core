using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class ChildLeafInfo : List<RuntimeGroupLeafObjReference>, IStorable, IPersistable
	{
		private static Declaration m_declaration = GetDeclaration();

		public int Size => ItemSizes.SizeOf((List<RuntimeGroupLeafObjReference>)this);

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.List)
				{
					writer.WriteRIFList(this);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.List)
				{
					reader.ReadListOfRIFObjects(this);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChildLeafInfo;
		}

		internal static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.List, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObjReference));
				return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChildLeafInfo, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
