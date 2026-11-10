using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing
{
	internal class GroupTreePartition : IPersistable
	{
		private List<IReference<ScopeInstance>> m_topLevelScopeInstances;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal bool IsEmpty => m_topLevelScopeInstances == null;

		internal List<IReference<ScopeInstance>> TopLevelScopeInstances => m_topLevelScopeInstances;

		internal GroupTreePartition()
		{
		}

		internal void AddTopLevelScopeInstance(IReference<ScopeInstance> instance)
		{
			if (m_topLevelScopeInstances == null)
			{
				m_topLevelScopeInstances = new List<IReference<ScopeInstance>>();
			}
			m_topLevelScopeInstances.Add(instance);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.TopLevelScopeInstances, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScopeInstanceReference));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GroupTreePartition, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.TopLevelScopeInstances)
				{
					writer.Write(m_topLevelScopeInstances);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.TopLevelScopeInstances)
				{
					m_topLevelScopeInstances = reader.ReadGenericListOfRIFObjects<IReference<ScopeInstance>>();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
			reader.ResolveReferences();
		}

		public virtual void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public virtual ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GroupTreePartition;
		}
	}
}
