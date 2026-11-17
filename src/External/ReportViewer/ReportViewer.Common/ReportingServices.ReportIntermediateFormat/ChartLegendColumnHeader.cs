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
	internal sealed class ChartLegendColumnHeader : ChartStyleContainer, IPersistable
	{
		private ExpressionInfo m_value;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ChartLegendColumnHeaderExprHost m_exprHost;

		internal ChartLegendColumnHeaderExprHost ExprHost => m_exprHost;

		internal ExpressionInfo Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
			}
		}

		internal ChartLegendColumnHeader()
		{
		}

		internal ChartLegendColumnHeader(Chart chart)
			: base(chart)
		{
		}

		internal void SetExprHost(ChartLegendColumnHeaderExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartLegendColumnHeaderStart();
			base.Initialize(context);
			if (m_value != null)
			{
				m_value.Initialize("Value", context);
				context.ExprHostBuilder.ChartLegendColumnHeaderValue(m_value);
			}
			context.ExprHostBuilder.ChartLegendColumnHeaderEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartLegendColumnHeader chartLegendColumnHeader = (ChartLegendColumnHeader)base.PublishClone(context);
			if (m_value != null)
			{
				chartLegendColumnHeader.m_value = (ExpressionInfo)m_value.PublishClone(context);
			}
			return chartLegendColumnHeader;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Value, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendColumnHeader, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		internal string EvaluateValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendColumnHeaderValueExpression(this, m_chart.Name);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Value)
				{
					writer.Write(m_value);
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
				if (memberName == MemberName.Value)
				{
					m_value = (ExpressionInfo)reader.ReadRIFObject();
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
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendColumnHeader;
		}
	}
}
