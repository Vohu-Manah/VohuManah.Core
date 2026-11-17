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
	internal sealed class GaugeTickMarks : TickMarkStyle, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_interval;

		private ExpressionInfo m_intervalOffset;

		internal ExpressionInfo Interval
		{
			get
			{
				return m_interval;
			}
			set
			{
				m_interval = value;
			}
		}

		internal ExpressionInfo IntervalOffset
		{
			get
			{
				return m_intervalOffset;
			}
			set
			{
				m_intervalOffset = value;
			}
		}

		internal GaugeTickMarks()
		{
		}

		internal GaugeTickMarks(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal void Initialize(InitializationContext context, bool isMajor)
		{
			context.ExprHostBuilder.GaugeTickMarksStart(isMajor);
			InitializeInternal(context);
			if (m_interval != null)
			{
				m_interval.Initialize("Interval", context);
				context.ExprHostBuilder.GaugeTickMarksInterval(m_interval);
			}
			if (m_intervalOffset != null)
			{
				m_intervalOffset.Initialize("IntervalOffset", context);
				context.ExprHostBuilder.GaugeTickMarksIntervalOffset(m_intervalOffset);
			}
			context.ExprHostBuilder.GaugeTickMarksEnd(isMajor);
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			GaugeTickMarks gaugeTickMarks = (GaugeTickMarks)base.PublishClone(context);
			if (m_interval != null)
			{
				gaugeTickMarks.m_interval = (ExpressionInfo)m_interval.PublishClone(context);
			}
			if (m_intervalOffset != null)
			{
				gaugeTickMarks.m_intervalOffset = (ExpressionInfo)m_intervalOffset.PublishClone(context);
			}
			return gaugeTickMarks;
		}

		internal void SetExprHost(GaugeTickMarksExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			SetExprHost((TickMarkStyleExprHost)exprHost, reportObjectModel);
			m_exprHost = exprHost;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Interval, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffset, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeTickMarks, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TickMarkStyle, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Interval:
					writer.Write(m_interval);
					break;
				case MemberName.IntervalOffset:
					writer.Write(m_intervalOffset);
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
				case MemberName.Interval:
					m_interval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalOffset:
					m_intervalOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeTickMarks;
		}

		internal double EvaluateInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeTickMarksIntervalExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateIntervalOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeTickMarksIntervalOffsetExpression(this, m_gaugePanel.Name);
		}
	}
}
