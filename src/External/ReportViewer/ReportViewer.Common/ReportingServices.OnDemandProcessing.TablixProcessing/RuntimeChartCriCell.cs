using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeChartCriCell : RuntimeCell
	{
		[NonSerialized]
		private static Declaration m_declaration = GetDeclaration();

		internal RuntimeChartCriCell()
		{
		}

		internal RuntimeChartCriCell(RuntimeChartCriGroupLeafObjReference owner, ReportingServices.ReportIntermediateFormat.ReportHierarchyNode outerGroupingMember, ReportingServices.ReportIntermediateFormat.ReportHierarchyNode innerGroupingMember, bool innermost)
			: base(owner, outerGroupingMember, innerGroupingMember, innermost)
		{
		}

		protected override void ConstructCellContents(Cell cell, ref DataActions dataAction)
		{
		}

		protected override void CreateInstanceCellContents(Cell cell, DataCellInstance cellInstance, OnDemandProcessingContext odpContext)
		{
		}

		public override IOnDemandMemberOwnerInstanceReference GetDataRegionInstance(ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			Global.Tracer.Assert(condition: false, "This type of cell does not support nested data regions.");
			throw new InvalidOperationException();
		}

		public override IReference<IDataCorrelation> GetIdcReceiver(IRIFReportDataScope scope)
		{
			Global.Tracer.Assert(condition: false, "This type of cell does not support nested data regions.");
			throw new InvalidOperationException();
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
			Global.Tracer.Assert(condition: false, "RuntimeChartCriCell should not resolve references");
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriCell;
		}

		internal new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriCell, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCell, memberInfoList);
			}
			return m_declaration;
		}
	}
}
