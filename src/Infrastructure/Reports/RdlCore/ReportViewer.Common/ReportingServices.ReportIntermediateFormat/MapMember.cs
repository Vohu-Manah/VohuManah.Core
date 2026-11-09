using ReportingServices.RdlExpressions;
using ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapMember : ReportHierarchyNode, IPersistable
	{
		[NonSerialized]
		private MapMemberList m_innerMembers;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal override string RdlElementName => "MapMember";

		internal override HierarchyNodeList InnerHierarchy => m_innerMembers;

		internal MapMember ChildMapMember
		{
			get
			{
				if (m_innerMembers != null && m_innerMembers.Count == 1)
				{
					return m_innerMembers[0];
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					if (m_innerMembers == null)
					{
						m_innerMembers = new MapMemberList();
					}
					else
					{
						m_innerMembers.Clear();
					}
					m_innerMembers.Add(value);
				}
			}
		}

		internal MapMember()
		{
		}

		internal MapMember(int id, MapDataRegion crItem)
			: base(id, crItem)
		{
		}

		internal void SetIsCategoryMember(bool value)
		{
			m_isColumn = value;
			if (ChildMapMember != null)
			{
				ChildMapMember.SetIsCategoryMember(value);
			}
		}

		protected override void DataGroupStart(ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			builder.DataGroupStart(ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.MapDataRegion, m_isColumn);
		}

		protected override int DataGroupEnd(ReportingServices.RdlExpressions.ExprHostBuilder builder)
		{
			return builder.DataGroupEnd(ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.MapDataRegion, m_isColumn);
		}

		private List<MapVectorLayer> GetChildMapLayers()
		{
			return ((MapDataRegion)base.DataRegionDef).GetChildVectorLayers();
		}

		internal override bool InnerInitialize(InitializationContext context, bool restrictive)
		{
			foreach (MapVectorLayer childMapLayer in GetChildMapLayers())
			{
				childMapLayer.InitializeMapMember(context);
			}
			return base.InnerInitialize(context, restrictive);
		}

		internal override bool Initialize(InitializationContext context, bool restrictive)
		{
			if (!m_isColumn)
			{
				if (m_grouping != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidRowMapMemberCannotBeDynamic, Severity.Error, ReportingServices.ReportProcessing.ObjectType.MapDataRegion, context.TablixName, "MapMember", "Group", m_grouping.Name);
				}
				if (m_innerMembers != null)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsInvalidRowMapMemberCannotContainChildMember, Severity.Error, ReportingServices.ReportProcessing.ObjectType.MapDataRegion, context.TablixName, "MapMember");
				}
			}
			else if (m_innerMembers != null && m_innerMembers.Count > 1)
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsInvalidColumnMapMemberCannotContainMultipleChildMember, Severity.Error, ReportingServices.ReportProcessing.ObjectType.MapDataRegion, context.TablixName, "MapMember");
			}
			return base.Initialize(context, restrictive);
		}

		internal override object PublishClone(AutomaticSubtotalContext context, DataRegion newContainingRegion)
		{
			MapMember mapMember = (MapMember)base.PublishClone(context, newContainingRegion);
			if (ChildMapMember != null)
			{
				mapMember.ChildMapMember = (MapMember)ChildMapMember.PublishClone(context, newContainingRegion);
			}
			return mapMember;
		}

		[SkipMemberStaticValidation(MemberName.MapMember)]
		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapMember, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMember));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMember, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportHierarchyNode, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.MapMember)
				{
					writer.Write(ChildMapMember);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.MapMember)
				{
					ChildMapMember = (MapMember)reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMember;
		}

		internal override void SetExprHost(IMemberNode memberExprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(memberExprHost != null && reportObjectModel != null);
			MemberNodeSetExprHost(memberExprHost, reportObjectModel);
			List<MapVectorLayer> childMapLayers = GetChildMapLayers();
			MapMemberExprHost obj = (MapMemberExprHost)memberExprHost;
			IList<MapPolygonLayerExprHost> mapPolygonLayersHostsRemotable = obj.MapPolygonLayersHostsRemotable;
			IList<MapPointLayerExprHost> mapPointLayersHostsRemotable = obj.MapPointLayersHostsRemotable;
			IList<MapLineLayerExprHost> mapLineLayersHostsRemotable = obj.MapLineLayersHostsRemotable;
			if (childMapLayers == null)
			{
				return;
			}
			for (int i = 0; i < childMapLayers.Count; i++)
			{
				MapVectorLayer mapVectorLayer = childMapLayers[i];
				if (mapVectorLayer == null || mapVectorLayer.ExpressionHostMapMemberID <= -1)
				{
					continue;
				}
				if (mapVectorLayer is MapPolygonLayer)
				{
					if (mapPolygonLayersHostsRemotable != null)
					{
						mapVectorLayer.SetExprHostMapMember(mapPolygonLayersHostsRemotable[mapVectorLayer.ExpressionHostMapMemberID], reportObjectModel);
					}
				}
				else if (mapVectorLayer is MapPointLayer)
				{
					if (mapPointLayersHostsRemotable != null)
					{
						mapVectorLayer.SetExprHostMapMember(mapPointLayersHostsRemotable[mapVectorLayer.ExpressionHostMapMemberID], reportObjectModel);
					}
				}
				else if (mapVectorLayer is MapLineLayer && mapLineLayersHostsRemotable != null)
				{
					mapVectorLayer.SetExprHostMapMember(mapLineLayersHostsRemotable[mapVectorLayer.ExpressionHostMapMemberID], reportObjectModel);
				}
			}
		}

		internal override void MemberContentsSetExprHost(ObjectModelImpl reportObjectModel, bool traverseDataRegions)
		{
		}
	}
}
