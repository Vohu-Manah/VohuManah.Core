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
	internal sealed class MapColorRangeRule : MapColorRule, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_startColor;

		private ExpressionInfo m_middleColor;

		private ExpressionInfo m_endColor;

		internal ExpressionInfo StartColor
		{
			get
			{
				return m_startColor;
			}
			set
			{
				m_startColor = value;
			}
		}

		internal ExpressionInfo MiddleColor
		{
			get
			{
				return m_middleColor;
			}
			set
			{
				m_middleColor = value;
			}
		}

		internal ExpressionInfo EndColor
		{
			get
			{
				return m_endColor;
			}
			set
			{
				m_endColor = value;
			}
		}

		internal new MapColorRangeRuleExprHost ExprHost => (MapColorRangeRuleExprHost)m_exprHost;

		internal MapColorRangeRule()
		{
		}

		internal MapColorRangeRule(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapColorRangeRuleStart();
			base.Initialize(context);
			if (m_startColor != null)
			{
				m_startColor.Initialize("StartColor", context);
				context.ExprHostBuilder.MapColorRangeRuleStartColor(m_startColor);
			}
			if (m_middleColor != null)
			{
				m_middleColor.Initialize("MiddleColor", context);
				context.ExprHostBuilder.MapColorRangeRuleMiddleColor(m_middleColor);
			}
			if (m_endColor != null)
			{
				m_endColor.Initialize("EndColor", context);
				context.ExprHostBuilder.MapColorRangeRuleEndColor(m_endColor);
			}
			context.ExprHostBuilder.MapColorRangeRuleEnd();
		}

		internal override void InitializeMapMember(InitializationContext context)
		{
			context.ExprHostBuilder.MapColorRangeRuleStart();
			base.InitializeMapMember(context);
			context.ExprHostBuilder.MapColorRangeRuleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapColorRangeRule mapColorRangeRule = (MapColorRangeRule)base.PublishClone(context);
			if (m_startColor != null)
			{
				mapColorRangeRule.m_startColor = (ExpressionInfo)m_startColor.PublishClone(context);
			}
			if (m_middleColor != null)
			{
				mapColorRangeRule.m_middleColor = (ExpressionInfo)m_middleColor.PublishClone(context);
			}
			if (m_endColor != null)
			{
				mapColorRangeRule.m_endColor = (ExpressionInfo)m_endColor.PublishClone(context);
			}
			return mapColorRangeRule;
		}

		internal override void SetExprHost(MapAppearanceRuleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.StartColor, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MiddleColor, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EndColor, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorRangeRule, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorRule, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.StartColor:
					writer.Write(m_startColor);
					break;
				case MemberName.MiddleColor:
					writer.Write(m_middleColor);
					break;
				case MemberName.EndColor:
					writer.Write(m_endColor);
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
				case MemberName.StartColor:
					m_startColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MiddleColor:
					m_middleColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EndColor:
					m_endColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorRangeRule;
		}

		internal string EvaluateStartColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorRangeRuleStartColorExpression(this, m_map.Name);
		}

		internal string EvaluateMiddleColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorRangeRuleMiddleColorExpression(this, m_map.Name);
		}

		internal string EvaluateEndColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorRangeRuleEndColorExpression(this, m_map.Name);
		}
	}
}
