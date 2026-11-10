using ReportingServices.RdlExpressions;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapCell : Cell, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		protected override bool IsDataRegionBodyCell => true;

		public ReportingServices.ReportProcessing.ObjectType ObjectType => ReportingServices.ReportProcessing.ObjectType.MapCell;

		public override ReportingServices.ReportProcessing.ObjectType DataScopeObjectType => ObjectType;

		protected override ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode ExprHostDataRegionMode => ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.MapDataRegion;

		internal MapCell()
		{
		}

		internal MapCell(int id, MapDataRegion mapDataRegion)
			: base(id, mapDataRegion)
		{
		}

		internal override void InternalInitialize(int parentRowID, int parentColumnID, int rowindex, int colIndex, InitializationContext context)
		{
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> memberInfoList = new List<MemberInfo>();
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCell, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Cell, memberInfoList);
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCell;
		}
	}
}
