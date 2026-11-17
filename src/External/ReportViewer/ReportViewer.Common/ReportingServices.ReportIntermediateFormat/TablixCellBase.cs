using ReportingServices.RdlExpressions;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	internal abstract class TablixCellBase : Cell, IPersistable
	{
		protected int m_rowSpan;

		protected int m_colSpan;

		protected ReportItem m_cellContents;

		protected ReportItem m_altCellContents;

		[NonSerialized]
		private List<ReportItem> m_cellContentCollection;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal int ColSpan
		{
			get
			{
				return m_colSpan;
			}
			set
			{
				m_colSpan = value;
			}
		}

		internal int RowSpan
		{
			get
			{
				return m_rowSpan;
			}
			set
			{
				m_rowSpan = value;
			}
		}

		internal ReportItem CellContents
		{
			get
			{
				return m_cellContents;
			}
			set
			{
				m_cellContents = value;
			}
		}

		internal ReportItem AltCellContents
		{
			get
			{
				return m_altCellContents;
			}
			set
			{
				m_altCellContents = value;
			}
		}

		internal override List<ReportItem> CellContentCollection
		{
			get
			{
				if (m_cellContentCollection == null && m_hasInnerGroupTreeHierarchy && m_cellContents != null)
				{
					m_cellContentCollection = new List<ReportItem>((m_altCellContents == null) ? 1 : 2);
					if (m_cellContents != null)
					{
						m_cellContentCollection.Add(m_cellContents);
					}
					if (m_altCellContents != null)
					{
						m_cellContentCollection.Add(m_altCellContents);
					}
				}
				return m_cellContentCollection;
			}
		}

		public override ReportingServices.ReportProcessing.ObjectType DataScopeObjectType => ReportingServices.ReportProcessing.ObjectType.TablixCell;

		protected override ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode ExprHostDataRegionMode => ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Tablix;

		internal TablixCellBase()
		{
		}

		internal TablixCellBase(int id, DataRegion dataRegion)
			: base(id, dataRegion)
		{
		}

		protected override void TraverseNestedScopes(IRIFScopeVisitor visitor)
		{
			if (m_cellContents != null)
			{
				m_cellContents.TraverseScopes(visitor);
			}
			if (m_altCellContents != null)
			{
				m_altCellContents.TraverseScopes(visitor);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			TablixCellBase tablixCellBase = (TablixCellBase)base.PublishClone(context);
			if (m_cellContents != null)
			{
				tablixCellBase.m_cellContents = (ReportItem)m_cellContents.PublishClone(context);
				if (m_altCellContents != null)
				{
					Global.Tracer.Assert(tablixCellBase.m_cellContents is CustomReportItem);
					tablixCellBase.m_altCellContents = (ReportItem)m_altCellContents.PublishClone(context);
					((CustomReportItem)tablixCellBase.m_cellContents).AltReportItem = tablixCellBase.m_altCellContents;
				}
			}
			return tablixCellBase;
		}

		internal override void InternalInitialize(int parentRowID, int parentColumnID, int rowindex, int colIndex, InitializationContext context)
		{
			if (m_cellContents != null)
			{
				if (IsDataRegionBodyCell)
				{
					context.IsTopLevelCellContents = true;
				}
				m_cellContents.Initialize(context);
				DataRendererInitialize(context);
				if (m_altCellContents != null)
				{
					m_altCellContents.Initialize(context);
				}
				m_hasInnerGroupTreeHierarchy = (Cell.ContainsInnerGroupTreeHierarchy(m_cellContents) | Cell.ContainsInnerGroupTreeHierarchy(m_altCellContents));
			}
		}

		internal virtual void DataRendererInitialize(InitializationContext context)
		{
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.RowSpan, Token.Int32));
			list.Add(new MemberInfo(MemberName.ColSpan, Token.Int32));
			list.Add(new MemberInfo(MemberName.CellContents, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem));
			list.Add(new MemberInfo(MemberName.AltCellContents, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCellBase, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Cell, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.RowSpan:
					writer.Write(m_rowSpan);
					break;
				case MemberName.ColSpan:
					writer.Write(m_colSpan);
					break;
				case MemberName.CellContents:
					writer.Write(m_cellContents);
					break;
				case MemberName.AltCellContents:
					writer.Write(m_altCellContents);
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
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.RowSpan:
					m_rowSpan = reader.ReadInt32();
					break;
				case MemberName.ColSpan:
					m_colSpan = reader.ReadInt32();
					break;
				case MemberName.CellContents:
					m_cellContents = (ReportItem)reader.ReadRIFObject();
					break;
				case MemberName.AltCellContents:
					m_altCellContents = (ReportItem)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TablixCellBase;
		}
	}
}
