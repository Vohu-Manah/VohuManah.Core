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
	internal sealed class MapCustomColor : IPersistable
	{
		private int m_exprHostID = -1;

		[NonSerialized]
		private MapCustomColorExprHost m_exprHost;

		[Reference]
		private Map m_map;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_color;

		internal ExpressionInfo Color
		{
			get
			{
				return m_color;
			}
			set
			{
				m_color = value;
			}
		}

		internal string OwnerName => m_map.Name;

		internal MapCustomColorExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal MapCustomColor()
		{
		}

		internal MapCustomColor(Map map)
		{
			m_map = map;
		}

		internal void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.MapCustomColorStart(index.ToString(CultureInfo.InvariantCulture.NumberFormat));
			if (m_color != null)
			{
				m_color.Initialize("Color", context);
				context.ExprHostBuilder.MapCustomColorColor(m_color);
			}
			m_exprHostID = context.ExprHostBuilder.MapCustomColorEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			MapCustomColor mapCustomColor = (MapCustomColor)MemberwiseClone();
			mapCustomColor.m_map = context.CurrentMapClone;
			if (m_color != null)
			{
				mapCustomColor.m_color = (ExpressionInfo)m_color.PublishClone(context);
			}
			return mapCustomColor;
		}

		internal void SetExprHost(MapCustomColorExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Color, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Map, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCustomColor, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
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
				case MemberName.Color:
					writer.Write(m_color);
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
				case MemberName.Color:
					m_color = (ExpressionInfo)reader.ReadRIFObject();
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
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCustomColor;
		}

		internal string EvaluateColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapCustomColorColorExpression(this, m_map.Name);
		}
	}
}
