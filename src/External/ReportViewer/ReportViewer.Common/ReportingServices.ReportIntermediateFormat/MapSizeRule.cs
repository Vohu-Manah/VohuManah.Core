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
	internal sealed class MapSizeRule : MapAppearanceRule, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_startSize;

		private ExpressionInfo m_endSize;

		internal ExpressionInfo StartSize
		{
			get
			{
				return m_startSize;
			}
			set
			{
				m_startSize = value;
			}
		}

		internal ExpressionInfo EndSize
		{
			get
			{
				return m_endSize;
			}
			set
			{
				m_endSize = value;
			}
		}

		internal new MapSizeRuleExprHost ExprHost => (MapSizeRuleExprHost)m_exprHost;

		internal MapSizeRule()
		{
		}

		internal MapSizeRule(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapSizeRuleStart();
			base.Initialize(context);
			if (m_startSize != null)
			{
				m_startSize.Initialize("StartSize", context);
				context.ExprHostBuilder.MapSizeRuleStartSize(m_startSize);
			}
			if (m_endSize != null)
			{
				m_endSize.Initialize("EndSize", context);
				context.ExprHostBuilder.MapSizeRuleEndSize(m_endSize);
			}
			context.ExprHostBuilder.MapSizeRuleEnd();
		}

		internal override void InitializeMapMember(InitializationContext context)
		{
			context.ExprHostBuilder.MapSizeRuleStart();
			base.InitializeMapMember(context);
			context.ExprHostBuilder.MapSizeRuleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapSizeRule mapSizeRule = (MapSizeRule)base.PublishClone(context);
			if (m_startSize != null)
			{
				mapSizeRule.m_startSize = (ExpressionInfo)m_startSize.PublishClone(context);
			}
			if (m_endSize != null)
			{
				mapSizeRule.m_endSize = (ExpressionInfo)m_endSize.PublishClone(context);
			}
			return mapSizeRule;
		}

		internal override void SetExprHost(MapAppearanceRuleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.StartSize, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EndSize, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSizeRule, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapAppearanceRule, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.StartSize:
					writer.Write(m_startSize);
					break;
				case MemberName.EndSize:
					writer.Write(m_endSize);
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
				case MemberName.StartSize:
					m_startSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EndSize:
					m_endSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSizeRule;
		}

		internal string EvaluateStartSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSizeRuleStartSizeExpression(this, m_map.Name);
		}

		internal string EvaluateEndSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSizeRuleEndSizeExpression(this, m_map.Name);
		}
	}
}
