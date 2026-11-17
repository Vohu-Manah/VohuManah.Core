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
	internal sealed class MapMarkerRule : MapAppearanceRule, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private List<MapMarker> m_mapMarkers;

		internal List<MapMarker> MapMarkers
		{
			get
			{
				return m_mapMarkers;
			}
			set
			{
				m_mapMarkers = value;
			}
		}

		internal new MapMarkerRuleExprHost ExprHost => (MapMarkerRuleExprHost)m_exprHost;

		internal MapMarkerRule()
		{
		}

		internal MapMarkerRule(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapMarkerRuleStart();
			base.Initialize(context);
			if (m_mapMarkers != null)
			{
				for (int i = 0; i < m_mapMarkers.Count; i++)
				{
					m_mapMarkers[i].Initialize(context, i);
				}
			}
			context.ExprHostBuilder.MapMarkerRuleEnd();
		}

		internal override void InitializeMapMember(InitializationContext context)
		{
			context.ExprHostBuilder.MapMarkerRuleStart();
			base.InitializeMapMember(context);
			context.ExprHostBuilder.MapMarkerRuleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapMarkerRule mapMarkerRule = (MapMarkerRule)base.PublishClone(context);
			if (m_mapMarkers != null)
			{
				mapMarkerRule.m_mapMarkers = new List<MapMarker>(m_mapMarkers.Count);
				{
					foreach (MapMarker mapMarker in m_mapMarkers)
					{
						mapMarkerRule.m_mapMarkers.Add((MapMarker)mapMarker.PublishClone(context));
					}
					return mapMarkerRule;
				}
			}
			return mapMarkerRule;
		}

		internal override void SetExprHost(MapAppearanceRuleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			IList<MapMarkerExprHost> mapMarkersHostsRemotable = ExprHost.MapMarkersHostsRemotable;
			if (m_mapMarkers == null || mapMarkersHostsRemotable == null)
			{
				return;
			}
			for (int i = 0; i < m_mapMarkers.Count; i++)
			{
				MapMarker mapMarker = m_mapMarkers[i];
				if (mapMarker != null && mapMarker.ExpressionHostID > -1)
				{
					mapMarker.SetExprHost(mapMarkersHostsRemotable[mapMarker.ExpressionHostID], reportObjectModel);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapMarkers, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarker));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarkerRule, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapAppearanceRule, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.MapMarkers)
				{
					writer.Write(m_mapMarkers);
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
				if (memberName == MemberName.MapMarkers)
				{
					m_mapMarkers = reader.ReadGenericListOfRIFObjects<MapMarker>();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarkerRule;
		}
	}
}
