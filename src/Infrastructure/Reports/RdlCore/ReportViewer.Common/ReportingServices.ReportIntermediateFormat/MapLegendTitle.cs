using ReportingServices.OnDemandProcessing;
using ReportingServices.OnDemandReportRendering;
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
	internal sealed class MapLegendTitle : MapStyleContainer, IPersistable
	{
		[NonSerialized]
		private MapLegendTitleExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_caption;

		private ExpressionInfo m_titleSeparator;

		private ExpressionInfo m_titleSeparatorColor;

		internal ExpressionInfo Caption
		{
			get
			{
				return m_caption;
			}
			set
			{
				m_caption = value;
			}
		}

		internal ExpressionInfo TitleSeparator
		{
			get
			{
				return m_titleSeparator;
			}
			set
			{
				m_titleSeparator = value;
			}
		}

		internal ExpressionInfo TitleSeparatorColor
		{
			get
			{
				return m_titleSeparatorColor;
			}
			set
			{
				m_titleSeparatorColor = value;
			}
		}

		internal string OwnerName => m_map.Name;

		internal MapLegendTitleExprHost ExprHost => m_exprHost;

		internal MapLegendTitle()
		{
		}

		internal MapLegendTitle(Map map)
			: base(map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapLegendTitleStart();
			base.Initialize(context);
			if (m_caption != null)
			{
				m_caption.Initialize("Caption", context);
				context.ExprHostBuilder.MapLegendTitleCaption(m_caption);
			}
			if (m_titleSeparator != null)
			{
				m_titleSeparator.Initialize("TitleSeparator", context);
				context.ExprHostBuilder.MapLegendTitleTitleSeparator(m_titleSeparator);
			}
			if (m_titleSeparatorColor != null)
			{
				m_titleSeparatorColor.Initialize("TitleSeparatorColor", context);
				context.ExprHostBuilder.MapLegendTitleTitleSeparatorColor(m_titleSeparatorColor);
			}
			context.ExprHostBuilder.MapLegendTitleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapLegendTitle mapLegendTitle = (MapLegendTitle)base.PublishClone(context);
			if (m_caption != null)
			{
				mapLegendTitle.m_caption = (ExpressionInfo)m_caption.PublishClone(context);
			}
			if (m_titleSeparator != null)
			{
				mapLegendTitle.m_titleSeparator = (ExpressionInfo)m_titleSeparator.PublishClone(context);
			}
			if (m_titleSeparatorColor != null)
			{
				mapLegendTitle.m_titleSeparatorColor = (ExpressionInfo)m_titleSeparatorColor.PublishClone(context);
			}
			return mapLegendTitle;
		}

		internal void SetExprHost(MapLegendTitleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Caption, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TitleSeparator, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TitleSeparatorColor, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLegendTitle, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Caption:
					writer.Write(m_caption);
					break;
				case MemberName.TitleSeparator:
					writer.Write(m_titleSeparator);
					break;
				case MemberName.TitleSeparatorColor:
					writer.Write(m_titleSeparatorColor);
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
				case MemberName.Caption:
					m_caption = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TitleSeparator:
					m_titleSeparator = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TitleSeparatorColor:
					m_titleSeparatorColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLegendTitle;
		}

		internal string EvaluateCaption(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			ReportingServices.RdlExpressions.VariantResult result = context.ReportRuntime.EvaluateMapLegendTitleCaptionExpression(this, m_map.Name);
			return m_map.GetFormattedStringFromValue(ref result, context);
		}

		internal MapLegendTitleSeparator EvaluateTitleSeparator(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapLegendTitleSeparator(context.ReportRuntime.EvaluateMapLegendTitleTitleSeparatorExpression(this, m_map.Name), context.ReportRuntime);
		}

		internal string EvaluateTitleSeparatorColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLegendTitleTitleSeparatorColorExpression(this, m_map.Name);
		}
	}
}
