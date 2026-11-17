using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	internal sealed class TablixCornerCell : TablixCellBase, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal TablixCornerCell()
		{
		}

		internal TablixCornerCell(int id, DataRegion dataRegion)
			: base(id, dataRegion)
		{
		}

		internal override void InternalInitialize(int parentRowID, int parentColumnID, int rowindex, int colIndex, InitializationContext context)
		{
			base.InternalInitialize(parentRowID, parentColumnID, rowindex, colIndex, context);
			m_hasInnerGroupTreeHierarchy = (Cell.ContainsInnerGroupTreeHierarchy(m_cellContents) | Cell.ContainsInnerGroupTreeHierarchy(m_altCellContents));
		}

		protected override void StartExprHost(InitializationContext context)
		{
		}

		protected override void EndExprHost(InitializationContext context)
		{
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			return (TablixCornerCell)base.PublishClone(context);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> memberInfoList = new List<MemberInfo>();
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCornerCell, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCellBase, memberInfoList);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				_ = writer.CurrentMember.MemberName;
				Global.Tracer.Assert(condition: false);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
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
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCornerCell;
		}
	}
}
