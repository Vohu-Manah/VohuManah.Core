using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeTablixGroupLeafObj : RuntimeDataTablixWithScopedItemsGroupLeafObj
	{
		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal RuntimeTablixGroupLeafObj()
		{
		}

		internal RuntimeTablixGroupLeafObj(RuntimeDataTablixGroupRootObjReference groupRootRef, ReportingServices.ReportProcessing.ObjectType objectType)
			: base(groupRootRef, objectType)
		{
		}

		protected override List<ReportingServices.ReportIntermediateFormat.ReportItem> GetGroupScopedContents(ReportingServices.ReportIntermediateFormat.ReportHierarchyNode member)
		{
			return ((TablixMember)member).GroupScopedContentsForProcessing;
		}

		protected override List<ReportingServices.ReportIntermediateFormat.ReportItem> GetCellContents(Cell cell)
		{
			TablixCell tablixCell = cell as TablixCell;
			if (tablixCell != null && tablixCell.HasInnerGroupTreeHierarchy)
			{
				return tablixCell.CellContentCollection;
			}
			return null;
		}

		protected override RuntimeCell CreateCell(ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember)
		{
			return new RuntimeTablixCell((RuntimeTablixGroupLeafObjReference)m_selfReference, (TablixMember)outerGroupingMember, (TablixMember)innerGroupingMember, !m_hasInnerHierarchy);
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixGroupLeafObj;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
		}

		public override void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				m_declaration = new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixGroupLeafObj, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixWithScopedItemsGroupLeafObj, memberInfoList);
			}
			return m_declaration;
		}
	}
}
