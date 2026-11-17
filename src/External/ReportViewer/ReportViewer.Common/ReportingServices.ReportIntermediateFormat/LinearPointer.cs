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
	internal sealed class LinearPointer : GaugePointer, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_type;

		private Thermometer m_thermometer;

		internal ExpressionInfo Type
		{
			get
			{
				return m_type;
			}
			set
			{
				m_type = value;
			}
		}

		internal Thermometer Thermometer
		{
			get
			{
				return m_thermometer;
			}
			set
			{
				m_thermometer = value;
			}
		}

		internal LinearPointer()
		{
		}

		internal LinearPointer(GaugePanel gaugePanel, int id)
			: base(gaugePanel, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.LinearPointerStart(m_name);
			base.Initialize(context);
			if (m_type != null)
			{
				m_type.Initialize("Type", context);
				context.ExprHostBuilder.LinearPointerType(m_type);
			}
			if (m_thermometer != null)
			{
				m_thermometer.Initialize(context);
			}
			m_exprHostID = context.ExprHostBuilder.LinearPointerEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			LinearPointer linearPointer = (LinearPointer)base.PublishClone(context);
			if (m_type != null)
			{
				linearPointer.m_type = (ExpressionInfo)m_type.PublishClone(context);
			}
			if (m_thermometer != null)
			{
				linearPointer.m_thermometer = (Thermometer)m_thermometer.PublishClone(context);
			}
			return linearPointer;
		}

		internal void SetExprHost(LinearPointerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			SetExprHost((GaugePointerExprHost)exprHost, reportObjectModel);
			m_exprHost = exprHost;
			if (m_thermometer != null && ((LinearPointerExprHost)m_exprHost).ThermometerHost != null)
			{
				m_thermometer.SetExprHost(((LinearPointerExprHost)m_exprHost).ThermometerHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Type, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Thermometer, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Thermometer));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearPointer, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePointer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Type:
					writer.Write(m_type);
					break;
				case MemberName.Thermometer:
					writer.Write(m_thermometer);
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
				case MemberName.Type:
					m_type = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Thermometer:
					m_thermometer = (Thermometer)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LinearPointer;
		}

		internal LinearPointerTypes EvaluateType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateLinearPointerTypes(context.ReportRuntime.EvaluateLinearPointerTypeExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}
	}
}
