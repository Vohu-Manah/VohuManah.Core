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
	internal sealed class MapColorScaleTitle : MapStyleContainer, IPersistable
	{
		[NonSerialized]
		private MapColorScaleTitleExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_caption;

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

		internal string OwnerName => m_map.Name;

		internal MapColorScaleTitleExprHost ExprHost => m_exprHost;

		internal MapColorScaleTitle()
		{
		}

		internal MapColorScaleTitle(Map map)
			: base(map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapColorScaleTitleStart();
			base.Initialize(context);
			if (m_caption != null)
			{
				m_caption.Initialize("Caption", context);
				context.ExprHostBuilder.MapColorScaleTitleCaption(m_caption);
			}
			context.ExprHostBuilder.MapColorScaleTitleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapColorScaleTitle mapColorScaleTitle = (MapColorScaleTitle)base.PublishClone(context);
			if (m_caption != null)
			{
				mapColorScaleTitle.m_caption = (ExpressionInfo)m_caption.PublishClone(context);
			}
			return mapColorScaleTitle;
		}

		internal void SetExprHost(MapColorScaleTitleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Caption, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorScaleTitle, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Caption)
				{
					writer.Write(m_caption);
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
				if (memberName == MemberName.Caption)
				{
					m_caption = (ExpressionInfo)reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorScaleTitle;
		}

		internal string EvaluateCaption(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			ReportingServices.RdlExpressions.VariantResult result = context.ReportRuntime.EvaluateMapColorScaleTitleCaptionExpression(this, m_map.Name);
			return m_map.GetFormattedStringFromValue(ref result, context);
		}
	}
}
