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
	internal sealed class FrameBackground : GaugePanelStyleContainer, IPersistable
	{
		[NonSerialized]
		private FrameBackgroundExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal string OwnerName => m_gaugePanel.Name;

		internal FrameBackgroundExprHost ExprHost => m_exprHost;

		internal FrameBackground()
		{
		}

		internal FrameBackground(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.FrameBackgroundStart();
			base.Initialize(context);
			context.ExprHostBuilder.FrameBackgroundEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			return (FrameBackground)base.PublishClone(context);
		}

		internal void SetExprHost(FrameBackgroundExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> memberInfoList = new List<MemberInfo>();
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FrameBackground, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, memberInfoList);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				_ = writer.CurrentMember.MemberName;
				Global.Tracer.Assert(condition: false);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				_ = reader.CurrentMember.MemberName;
				Global.Tracer.Assert(condition: false);
			}
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FrameBackground;
		}
	}
}
