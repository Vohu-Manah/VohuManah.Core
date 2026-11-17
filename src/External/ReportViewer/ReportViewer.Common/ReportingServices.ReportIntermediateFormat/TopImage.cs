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
	internal sealed class TopImage : BaseGaugeImage, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_hueColor;

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

		internal TopImage()
		{
		}

		internal TopImage(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.TopImageStart();
			base.Initialize(context);
			if (m_hueColor != null)
			{
				m_hueColor.Initialize("HueColor", context);
				context.ExprHostBuilder.TopImageHueColor(m_hueColor);
			}
			context.ExprHostBuilder.TopImageEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			TopImage topImage = (TopImage)base.PublishClone(context);
			if (m_hueColor != null)
			{
				topImage.m_hueColor = (ExpressionInfo)m_hueColor.PublishClone(context);
			}
			return topImage;
		}

		internal void SetExprHost(TopImageExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			SetExprHost((BaseGaugeImageExprHost)exprHost, reportObjectModel);
			m_exprHost = exprHost;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.HueColor, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TopImage, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BaseGaugeImage, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.HueColor)
				{
					writer.Write(m_hueColor);
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
				if (memberName == MemberName.HueColor)
				{
					m_hueColor = (ExpressionInfo)reader.ReadRIFObject();
				}
			}
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TopImage;
		}

		internal string EvaluateHueColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateTopImageHueColorExpression(this, m_gaugePanel.Name);
		}
	}
}
