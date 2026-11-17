using ReportingServices.OnDemandProcessing;
using ReportingServices.OnDemandReportRendering;
using ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapCustomView : MapView, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_centerX;

		private ExpressionInfo m_centerY;

		internal ExpressionInfo CenterX
		{
			get
			{
				return m_centerX;
			}
			set
			{
				m_centerX = value;
			}
		}

		internal ExpressionInfo CenterY
		{
			get
			{
				return m_centerY;
			}
			set
			{
				m_centerY = value;
			}
		}

		internal new MapCustomViewExprHost ExprHost => (MapCustomViewExprHost)m_exprHost;

		internal MapCustomView()
		{
		}

		internal MapCustomView(Map map)
			: base(map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapCustomViewStart();
			base.Initialize(context);
			if (m_centerX != null)
			{
				m_centerX.Initialize("CenterX", context);
				context.ExprHostBuilder.MapCustomViewCenterX(m_centerX);
			}
			if (m_centerY != null)
			{
				m_centerY.Initialize("CenterY", context);
				context.ExprHostBuilder.MapCustomViewCenterY(m_centerY);
			}
			context.ExprHostBuilder.MapCustomViewEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapCustomView mapCustomView = (MapCustomView)base.PublishClone(context);
			if (m_centerX != null)
			{
				mapCustomView.m_centerX = (ExpressionInfo)m_centerX.PublishClone(context);
			}
			if (m_centerY != null)
			{
				mapCustomView.m_centerY = (ExpressionInfo)m_centerY.PublishClone(context);
			}
			return mapCustomView;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.CenterX, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CenterY, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCustomView, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapView, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.CenterX:
					writer.Write(m_centerX);
					break;
				case MemberName.CenterY:
					writer.Write(m_centerY);
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
				case MemberName.CenterX:
					m_centerX = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CenterY:
					m_centerY = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapCustomView;
		}

		internal double EvaluateCenterX(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapCustomViewCenterXExpression(this, m_map.Name);
		}

		internal double EvaluateCenterY(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapCustomViewCenterYExpression(this, m_map.Name);
		}
	}
}
