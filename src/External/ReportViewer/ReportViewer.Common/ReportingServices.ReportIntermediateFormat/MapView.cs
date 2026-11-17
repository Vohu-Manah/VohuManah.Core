using ReportingServices.OnDemandProcessing;
using ReportingServices.OnDemandReportRendering;
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
	internal class MapView : IPersistable
	{
		[NonSerialized]
		protected MapViewExprHost m_exprHost;

		[Reference]
		protected Map m_map;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_zoom;

		internal ExpressionInfo Zoom
		{
			get
			{
				return m_zoom;
			}
			set
			{
				m_zoom = value;
			}
		}

		internal string OwnerName => m_map.Name;

		internal MapViewExprHost ExprHost => m_exprHost;

		internal MapView()
		{
		}

		internal MapView(Map map)
		{
			m_map = map;
		}

		internal virtual void Initialize(InitializationContext context)
		{
			if (m_zoom != null)
			{
				m_zoom.Initialize("Zoom", context);
				context.ExprHostBuilder.MapViewZoom(m_zoom);
			}
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context)
		{
			MapView mapView = (MapView)MemberwiseClone();
			mapView.m_map = context.CurrentMapClone;
			if (m_zoom != null)
			{
				mapView.m_zoom = (ExpressionInfo)m_zoom.PublishClone(context);
			}
			return mapView;
		}

		internal virtual void SetExprHost(MapViewExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Zoom, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Map, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapView, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Map:
					writer.WriteReference(m_map);
					break;
				case MemberName.Zoom:
					writer.Write(m_zoom);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Map:
					m_map = reader.ReadReference<Map>(this);
					break;
				case MemberName.Zoom:
					m_zoom = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.Map)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_map = (Map)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public virtual ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapView;
		}

		internal double EvaluateZoom(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapViewZoomExpression(this, m_map.Name);
		}
	}
}
