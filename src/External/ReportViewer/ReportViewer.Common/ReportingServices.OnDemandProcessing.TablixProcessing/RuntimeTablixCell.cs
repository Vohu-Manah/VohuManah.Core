using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeTablixCell : RuntimeCellWithContents
	{
		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal RuntimeTablixCell()
		{
		}

		internal RuntimeTablixCell(RuntimeTablixGroupLeafObjReference owner, TablixMember outerGroupingMember, TablixMember innerGroupingMember, bool innermost)
			: base(owner, outerGroupingMember, innerGroupingMember, innermost)
		{
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

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixCell;
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
			Global.Tracer.Assert(condition: false, GetType().Name + " should not resolve references");
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				m_declaration = new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixCell, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellWithContents, memberInfoList);
			}
			return m_declaration;
		}
	}
}
