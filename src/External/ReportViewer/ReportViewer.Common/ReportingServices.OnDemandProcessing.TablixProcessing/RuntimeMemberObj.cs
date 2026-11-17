using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal abstract class RuntimeMemberObj : IStorable, IPersistable
	{
		protected IReference<IScope> m_owner;

		protected RuntimeDataTablixGroupRootObjReference m_groupRoot;

		private static Declaration m_declaration = GetDeclaration();

		internal RuntimeDataTablixGroupRootObjReference GroupRoot => m_groupRoot;

		public virtual int Size => ItemSizes.SizeOf(m_owner) + ItemSizes.SizeOf(m_groupRoot);

		internal RuntimeMemberObj()
		{
		}

		internal RuntimeMemberObj(IReference<IScope> owner, ReportingServices.ReportIntermediateFormat.ReportHierarchyNode dynamicMember)
		{
			m_owner = owner;
		}

		internal virtual void NextRow(bool isOuterGrouping, OnDemandProcessingContext odpContext)
		{
			if (null != m_groupRoot)
			{
				using (m_groupRoot.PinValue())
				{
					m_groupRoot.Value().NextRow();
				}
			}
		}

		internal virtual bool SortAndFilter(AggregateUpdateContext aggContext)
		{
			if (null != m_groupRoot)
			{
				using (m_groupRoot.PinValue())
				{
					return m_groupRoot.Value().SortAndFilter(aggContext);
				}
			}
			return true;
		}

		internal virtual void UpdateAggregates(AggregateUpdateContext aggContext)
		{
			if (null != m_groupRoot)
			{
				using (m_groupRoot.PinValue())
				{
					m_groupRoot.Value().UpdateAggregates(aggContext);
				}
			}
		}

		internal virtual void CalculateRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCol, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			if (null != m_groupRoot)
			{
				using (m_groupRoot.PinValue())
				{
					m_groupRoot.Value().CalculateRunningValues(groupCol, lastGroup, aggContext);
				}
			}
		}

		internal virtual void PrepareCalculateRunningValues()
		{
			if (null != m_groupRoot)
			{
				using (m_groupRoot.PinValue())
				{
					m_groupRoot.Value().PrepareCalculateRunningValues();
				}
			}
		}

		internal abstract void CreateInstances(IReference<RuntimeDataRegionObj> outerGroupRef, OnDemandProcessingContext odpContext, DataRegionInstance dataRegionInstance, bool outerGroupings, IReference<RuntimeDataTablixGroupRootObj> currOuterGroupRoot, ScopeInstance parentInstance, IReference<RuntimeMemberObj>[] innerMembers, IReference<RuntimeDataTablixGroupLeafObj> innerGroupLeafRef);

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Owner:
					writer.Write(m_owner);
					break;
				case MemberName.GroupRoot:
					writer.Write(m_groupRoot);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Owner:
					m_owner = (IReference<IScope>)reader.ReadRIFObject();
					break;
				case MemberName.GroupRoot:
					m_groupRoot = (RuntimeDataTablixGroupRootObjReference)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public virtual ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObj;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Owner, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScopeReference));
				list.Add(new MemberInfo(MemberName.GroupRoot, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupRootObjReference));
				return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObj, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
