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
	internal sealed class IndicatorImage : BaseGaugeImage, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_hueColor;

		private ExpressionInfo m_transparency;

		internal ExpressionInfo HueColor
		{
			get
			{
				return m_hueColor;
			}
			set
			{
				m_hueColor = value;
			}
		}

		internal ExpressionInfo Transparency
		{
			get
			{
				return m_transparency;
			}
			set
			{
				m_transparency = value;
			}
		}

		internal new IndicatorImageExprHost ExprHost => (IndicatorImageExprHost)m_exprHost;

		internal IndicatorImage()
		{
		}

		internal IndicatorImage(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.IndicatorImageStart();
			base.Initialize(context);
			if (m_hueColor != null)
			{
				m_hueColor.Initialize("HueColor", context);
				context.ExprHostBuilder.IndicatorImageHueColor(m_hueColor);
			}
			if (m_transparency != null)
			{
				m_transparency.Initialize("Transparency", context);
				context.ExprHostBuilder.IndicatorImageTransparency(m_transparency);
			}
			context.ExprHostBuilder.IndicatorImageEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			IndicatorImage indicatorImage = (IndicatorImage)base.PublishClone(context);
			if (m_hueColor != null)
			{
				indicatorImage.m_hueColor = (ExpressionInfo)m_hueColor.PublishClone(context);
			}
			if (m_transparency != null)
			{
				indicatorImage.m_transparency = (ExpressionInfo)m_transparency.PublishClone(context);
			}
			return indicatorImage;
		}

		internal void SetExprHost(IndicatorImageExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			SetExprHost((BaseGaugeImageExprHost)exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.HueColor, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Transparency, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IndicatorImage, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BaseGaugeImage, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.HueColor:
					writer.Write(m_hueColor);
					break;
				case MemberName.Transparency:
					writer.Write(m_transparency);
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
				case MemberName.HueColor:
					m_hueColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Transparency:
					m_transparency = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IndicatorImage;
		}

		internal string EvaluateHueColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateIndicatorImageHueColorExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateTransparency(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateIndicatorImageTransparencyExpression(this, m_gaugePanel.Name);
		}
	}
}
