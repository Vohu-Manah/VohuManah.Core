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
	internal sealed class MapDistanceScale : MapDockableSubItem, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_scaleColor;

		private ExpressionInfo m_scaleBorderColor;

		internal ExpressionInfo ScaleColor
		{
			get
			{
				return m_scaleColor;
			}
			set
			{
				m_scaleColor = value;
			}
		}

		internal ExpressionInfo ScaleBorderColor
		{
			get
			{
				return m_scaleBorderColor;
			}
			set
			{
				m_scaleBorderColor = value;
			}
		}

		internal new MapDistanceScaleExprHost ExprHost => (MapDistanceScaleExprHost)m_exprHost;

		internal MapDistanceScale()
		{
		}

		internal MapDistanceScale(Map map, int id)
			: base(map, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapDistanceScaleStart();
			base.Initialize(context);
			if (m_scaleColor != null)
			{
				m_scaleColor.Initialize("ScaleColor", context);
				context.ExprHostBuilder.MapDistanceScaleScaleColor(m_scaleColor);
			}
			if (m_scaleBorderColor != null)
			{
				m_scaleBorderColor.Initialize("ScaleBorderColor", context);
				context.ExprHostBuilder.MapDistanceScaleScaleBorderColor(m_scaleBorderColor);
			}
			context.ExprHostBuilder.MapDistanceScaleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapDistanceScale mapDistanceScale = (MapDistanceScale)base.PublishClone(context);
			if (m_scaleColor != null)
			{
				mapDistanceScale.m_scaleColor = (ExpressionInfo)m_scaleColor.PublishClone(context);
			}
			if (m_scaleBorderColor != null)
			{
				mapDistanceScale.m_scaleBorderColor = (ExpressionInfo)m_scaleBorderColor.PublishClone(context);
			}
			return mapDistanceScale;
		}

		internal void SetExprHost(MapDistanceScaleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			SetExprHost((MapDockableSubItemExprHost)exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ScaleColor, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ScaleBorderColor, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDistanceScale, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDockableSubItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ScaleColor:
					writer.Write(m_scaleColor);
					break;
				case MemberName.ScaleBorderColor:
					writer.Write(m_scaleBorderColor);
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
				case MemberName.ScaleColor:
					m_scaleColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ScaleBorderColor:
					m_scaleBorderColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDistanceScale;
		}

		internal string EvaluateScaleColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapDistanceScaleScaleColorExpression(this, m_map.Name);
		}

		internal string EvaluateScaleBorderColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapDistanceScaleScaleBorderColorExpression(this, m_map.Name);
		}
	}
}
