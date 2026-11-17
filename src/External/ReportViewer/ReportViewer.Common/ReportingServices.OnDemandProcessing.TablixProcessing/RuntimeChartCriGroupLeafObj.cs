using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeChartCriGroupLeafObj : RuntimeDataTablixGroupLeafObj
	{
		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal RuntimeChartCriGroupLeafObj()
		{
		}

		internal RuntimeChartCriGroupLeafObj(RuntimeDataTablixGroupRootObjReference groupRootRef, ReportingServices.ReportProcessing.ObjectType objectType)
			: base(groupRootRef, objectType)
		{
		}

		internal override RuntimeDataTablixObjReference GetNestedDataRegion(ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			Global.Tracer.Assert(condition: false, "This type of group leaf does not support nested data regions.");
			throw new InvalidOperationException();
		}

		protected override void ConstructOutermostCellContents(Cell cell)
		{
		}

		internal override void CreateCell(RuntimeCells cellsCollection, int collectionKey, ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember, ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef)
		{
			RuntimeCell runtimeCell = new RuntimeChartCriCell((RuntimeChartCriGroupLeafObjReference)m_selfReference, outerGroupingMember, innerGroupingMember, !m_hasInnerHierarchy);
			if (runtimeCell.SelfReference == null)
			{
				cellsCollection.AddCell(collectionKey, runtimeCell);
				return;
			}
			IReference<RuntimeCell> selfReference = runtimeCell.SelfReference;
			selfReference.UnPinValue();
			cellsCollection.AddCell(collectionKey, selfReference);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				_ = writer.CurrentMember.MemberName;
				Global.Tracer.Assert(condition: false);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				_ = reader.CurrentMember.MemberName;
				Global.Tracer.Assert(condition: false);
			}
		}

		public override void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriGroupLeafObj;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriGroupLeafObj, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObj, memberInfoList);
			}
			return m_declaration;
		}
	}
}
