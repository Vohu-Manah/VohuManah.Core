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
	internal sealed class MapColorPaletteRule : MapColorRule, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_palette;

		internal ExpressionInfo Palette
		{
			get
			{
				return m_palette;
			}
			set
			{
				m_palette = value;
			}
		}

		internal new MapColorPaletteRuleExprHost ExprHost => (MapColorPaletteRuleExprHost)m_exprHost;

		internal MapColorPaletteRule()
		{
		}

		internal MapColorPaletteRule(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapColorPaletteRuleStart();
			base.Initialize(context);
			if (m_palette != null)
			{
				m_palette.Initialize("Palette", context);
				context.ExprHostBuilder.MapColorPaletteRulePalette(m_palette);
			}
			context.ExprHostBuilder.MapColorPaletteRuleEnd();
		}

		internal override void InitializeMapMember(InitializationContext context)
		{
			context.ExprHostBuilder.MapColorPaletteRuleStart();
			base.InitializeMapMember(context);
			context.ExprHostBuilder.MapColorPaletteRuleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapColorPaletteRule mapColorPaletteRule = (MapColorPaletteRule)base.PublishClone(context);
			if (m_palette != null)
			{
				mapColorPaletteRule.m_palette = (ExpressionInfo)m_palette.PublishClone(context);
			}
			return mapColorPaletteRule;
		}

		internal override void SetExprHost(MapAppearanceRuleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Palette, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorPaletteRule, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorRule, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Palette)
				{
					writer.Write(m_palette);
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
				if (memberName == MemberName.Palette)
				{
					m_palette = (ExpressionInfo)reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorPaletteRule;
		}

		internal MapPalette EvaluatePalette(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapPalette(context.ReportRuntime.EvaluateMapColorPaletteRulePaletteExpression(this, m_map.Name), context.ReportRuntime);
		}
	}
}
