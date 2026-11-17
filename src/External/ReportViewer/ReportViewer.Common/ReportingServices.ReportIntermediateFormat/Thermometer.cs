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
	internal sealed class Thermometer : GaugePanelStyleContainer, IPersistable
	{
		[NonSerialized]
		private ThermometerExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_bulbOffset;

		private ExpressionInfo m_bulbSize;

		private ExpressionInfo m_thermometerStyle;

		internal ExpressionInfo BulbOffset
		{
			get
			{
				return m_bulbOffset;
			}
			set
			{
				m_bulbOffset = value;
			}
		}

		internal ExpressionInfo BulbSize
		{
			get
			{
				return m_bulbSize;
			}
			set
			{
				m_bulbSize = value;
			}
		}

		internal ExpressionInfo ThermometerStyle
		{
			get
			{
				return m_thermometerStyle;
			}
			set
			{
				m_thermometerStyle = value;
			}
		}

		internal string OwnerName => m_gaugePanel.Name;

		internal ThermometerExprHost ExprHost => m_exprHost;

		internal Thermometer()
		{
		}

		internal Thermometer(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ThermometerStart();
			base.Initialize(context);
			if (m_bulbOffset != null)
			{
				m_bulbOffset.Initialize("BulbOffset", context);
				context.ExprHostBuilder.ThermometerBulbOffset(m_bulbOffset);
			}
			if (m_bulbSize != null)
			{
				m_bulbSize.Initialize("BulbSize", context);
				context.ExprHostBuilder.ThermometerBulbSize(m_bulbSize);
			}
			if (m_thermometerStyle != null)
			{
				m_thermometerStyle.Initialize("ThermometerStyle", context);
				context.ExprHostBuilder.ThermometerThermometerStyle(m_thermometerStyle);
			}
			context.ExprHostBuilder.ThermometerEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			Thermometer thermometer = (Thermometer)base.PublishClone(context);
			if (m_bulbOffset != null)
			{
				thermometer.m_bulbOffset = (ExpressionInfo)m_bulbOffset.PublishClone(context);
			}
			if (m_bulbSize != null)
			{
				thermometer.m_bulbSize = (ExpressionInfo)m_bulbSize.PublishClone(context);
			}
			if (m_thermometerStyle != null)
			{
				thermometer.m_thermometerStyle = (ExpressionInfo)m_thermometerStyle.PublishClone(context);
			}
			return thermometer;
		}

		internal void SetExprHost(ThermometerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.BulbOffset, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.BulbSize, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ThermometerStyle, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Thermometer, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.BulbOffset:
					writer.Write(m_bulbOffset);
					break;
				case MemberName.BulbSize:
					writer.Write(m_bulbSize);
					break;
				case MemberName.ThermometerStyle:
					writer.Write(m_thermometerStyle);
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
				case MemberName.BulbOffset:
					m_bulbOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.BulbSize:
					m_bulbSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ThermometerStyle:
					m_thermometerStyle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Thermometer;
		}

		internal double EvaluateBulbOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateThermometerBulbOffsetExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateBulbSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateThermometerBulbSizeExpression(this, m_gaugePanel.Name);
		}

		internal GaugeThermometerStyles EvaluateThermometerStyle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeThermometerStyles(context.ReportRuntime.EvaluateThermometerThermometerStyleExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}
	}
}
