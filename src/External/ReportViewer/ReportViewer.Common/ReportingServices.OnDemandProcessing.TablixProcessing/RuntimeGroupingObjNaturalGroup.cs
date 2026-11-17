using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeGroupingObjNaturalGroup : RuntimeGroupingObjLinkedList
	{
		private object m_lastValue;

		private IReference<RuntimeHierarchyObj> m_lastChild;

		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		public override int Size => base.Size + ItemSizes.SizeOf(m_lastValue) + ItemSizes.SizeOf(m_lastChild);

		internal RuntimeGroupingObjNaturalGroup()
		{
		}

		internal RuntimeGroupingObjNaturalGroup(RuntimeHierarchyObj owner, ReportingServices.ReportProcessing.ObjectType objectType)
			: base(owner, objectType)
		{
		}

		internal override void NextRow(object keyValue, bool hasParent, object parentKey)
		{
			if (m_lastChild != null && m_owner.OdpContext.EqualityComparer.Equals(m_lastValue, keyValue))
			{
				using (m_lastChild.PinValue())
				{
					m_lastChild.Value().NextRow();
				}
			}
			else
			{
				m_lastValue = keyValue;
				m_lastChild = CreateHierarchyObjAndAddToParent();
			}
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.LastValue:
					writer.Write(m_lastValue);
					break;
				case MemberName.LastChild:
					writer.Write(m_lastChild);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.LastValue:
					m_lastValue = reader.ReadVariant();
					break;
				case MemberName.LastChild:
					m_lastChild = (IReference<RuntimeHierarchyObj>)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjNaturalGroup;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.LastValue, Token.Object));
				list.Add(new MemberInfo(MemberName.LastChild, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObjReference));
				return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjNaturalGroup, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjLinkedList, list);
			}
			return m_declaration;
		}
	}
}
