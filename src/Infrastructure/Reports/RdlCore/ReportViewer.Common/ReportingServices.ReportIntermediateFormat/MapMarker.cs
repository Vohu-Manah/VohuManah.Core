using ReportingServices.OnDemandProcessing;
using ReportingServices.OnDemandReportRendering;
using ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapMarker : IPersistable
	{
		private int m_exprHostID = -1;

		[NonSerialized]
		private MapMarkerExprHost m_exprHost;

		[Reference]
		private Map m_map;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_mapMarkerStyle;

		private MapMarkerImage m_mapMarkerImage;

		internal ExpressionInfo MapMarkerStyle
		{
			get
			{
				return m_mapMarkerStyle;
			}
			set
			{
				m_mapMarkerStyle = value;
			}
		}

		internal MapMarkerImage MapMarkerImage
		{
			get
			{
				return m_mapMarkerImage;
			}
			set
			{
				m_mapMarkerImage = value;
			}
		}

		internal string OwnerName => m_map.Name;

		internal MapMarkerExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal MapMarker()
		{
		}

		internal MapMarker(Map map)
		{
			m_map = map;
		}

		internal void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.MapMarkerInCollectionStart(index.ToString(CultureInfo.InvariantCulture.NumberFormat));
			InnerInitialize(context);
			m_exprHostID = context.ExprHostBuilder.MapMarkerInCollectionEnd();
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapMarkerStart();
			InnerInitialize(context);
			context.ExprHostBuilder.MapMarkerEnd();
		}

		private void InnerInitialize(InitializationContext context)
		{
			if (m_mapMarkerStyle != null)
			{
				m_mapMarkerStyle.Initialize("MapMarkerStyle", context);
				context.ExprHostBuilder.MapMarkerMapMarkerStyle(m_mapMarkerStyle);
			}
			if (m_mapMarkerImage != null)
			{
				m_mapMarkerImage.Initialize(context);
			}
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			MapMarker mapMarker = (MapMarker)MemberwiseClone();
			mapMarker.m_map = context.CurrentMapClone;
			if (m_mapMarkerStyle != null)
			{
				mapMarker.m_mapMarkerStyle = (ExpressionInfo)m_mapMarkerStyle.PublishClone(context);
			}
			if (m_mapMarkerImage != null)
			{
				mapMarker.m_mapMarkerImage = (MapMarkerImage)m_mapMarkerImage.PublishClone(context);
			}
			return mapMarker;
		}

		internal void SetExprHost(MapMarkerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (m_mapMarkerImage != null && ExprHost.MapMarkerImageHost != null)
			{
				m_mapMarkerImage.SetExprHost(ExprHost.MapMarkerImageHost, reportObjectModel);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapMarkerStyle, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapMarkerlImage, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarkerImage));
			list.Add(new MemberInfo(MemberName.Map, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarker, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Map:
					writer.WriteReference(m_map);
					break;
				case MemberName.MapMarkerStyle:
					writer.Write(m_mapMarkerStyle);
					break;
				case MemberName.MapMarkerlImage:
					writer.Write(m_mapMarkerImage);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Map:
					m_map = reader.ReadReference<Map>(this);
					break;
				case MemberName.MapMarkerStyle:
					m_mapMarkerStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapMarkerlImage:
					m_mapMarkerImage = (MapMarkerImage)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
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

		public ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarker;
		}

		internal MapMarkerStyle EvaluateMapMarkerStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapMarkerStyle(context.ReportRuntime.EvaluateMapMarkerMapMarkerStyleExpression(this, m_map.Name), context.ReportRuntime);
		}
	}
}
