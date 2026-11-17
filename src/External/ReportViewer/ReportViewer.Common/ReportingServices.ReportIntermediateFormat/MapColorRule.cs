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
	internal class MapColorRule : MapAppearanceRule, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_showInColorScale;

		internal ExpressionInfo ShowInColorScale
		{
			get
			{
				return m_showInColorScale;
			}
			set
			{
				m_showInColorScale = value;
			}
		}

		internal new MapColorRuleExprHost ExprHost => (MapColorRuleExprHost)m_exprHost;

		internal MapColorRule()
		{
		}

		internal MapColorRule(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (m_showInColorScale != null)
			{
				m_showInColorScale.Initialize("ShowInColorScale", context);
				context.ExprHostBuilder.MapColorRuleShowInColorScale(m_showInColorScale);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapColorRule mapColorRule = (MapColorRule)base.PublishClone(context);
			if (m_showInColorScale != null)
			{
				mapColorRule.m_showInColorScale = (ExpressionInfo)m_showInColorScale.PublishClone(context);
			}
			return mapColorRule;
		}

		internal override void SetExprHost(MapAppearanceRuleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ShowInColorScale, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorRule, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapAppearanceRule, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.ShowInColorScale)
				{
					writer.Write(m_showInColorScale);
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
				if (memberName == MemberName.ShowInColorScale)
				{
					m_showInColorScale = (ExpressionInfo)reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorRule;
		}

		internal bool EvaluateShowInColorScale(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorRuleShowInColorScaleExpression(this, m_map.Name);
		}
	}
}
