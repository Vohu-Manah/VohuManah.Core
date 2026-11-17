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
	internal class ChartTitleBase : ChartStyleContainer, IPersistable
	{
		private ExpressionInfo m_caption;

		[NonSerialized]
		private Formatter m_formatter;

		[NonSerialized]
		private ChartTitleBaseExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

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

		internal ChartTitleBaseExprHost ExprHost => m_exprHost;

		internal ChartTitleBase()
		{
		}

		internal ChartTitleBase(Chart chart)
			: base(chart)
		{
			m_chart = chart;
		}

		internal override void SetExprHost(StyleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = (ChartTitleBaseExprHost)exprHost;
		}

		internal override void Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (m_caption != null)
			{
				m_caption.Initialize("Caption", context);
				context.ExprHostBuilder.ChartCaption(m_caption);
			}
		}

		internal string EvaluateCaption(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, instance);
			ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateChartTitleCaptionExpression(this, base.Name, "Caption");
			string result = null;
			if (variantResult.ErrorOccurred)
			{
				result = RPRes.rsExpressionErrorValue;
			}
			else if (variantResult.Value != null)
			{
				result = Formatter.Format(variantResult.Value, ref m_formatter, m_chart.StyleClass, m_styleClass, context, base.ObjectType, base.Name);
			}
			return result;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartTitleBase chartTitleBase = (ChartTitleBase)base.PublishClone(context);
			if (m_caption != null)
			{
				chartTitleBase.m_caption = (ExpressionInfo)m_caption.PublishClone(context);
			}
			return chartTitleBase;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Caption, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTitleBase, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
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

		public override void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTitleBase;
		}
	}
}
