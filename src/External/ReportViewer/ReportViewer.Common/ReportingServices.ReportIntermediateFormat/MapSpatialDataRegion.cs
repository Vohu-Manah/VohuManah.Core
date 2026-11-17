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
	internal sealed class MapSpatialDataRegion : MapSpatialData, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_vectorData;

		internal ExpressionInfo VectorData
		{
			get
			{
				return m_vectorData;
			}
			set
			{
				m_vectorData = value;
			}
		}

		internal new MapSpatialDataRegionExprHost ExprHost => (MapSpatialDataRegionExprHost)m_exprHost;

		private IInstancePath InstancePath => m_mapVectorLayer.InstancePath;

		internal MapSpatialDataRegion()
		{
		}

		internal MapSpatialDataRegion(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		internal override void InitializeMapMember(InitializationContext context)
		{
			context.ExprHostBuilder.MapSpatialDataRegionStart();
			base.InitializeMapMember(context);
			if (m_vectorData != null)
			{
				m_vectorData.Initialize("VectorData", context);
				context.ExprHostBuilder.MapSpatialDataRegionVectorData(m_vectorData);
			}
			context.ExprHostBuilder.MapSpatialDataRegionEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapSpatialDataRegion mapSpatialDataRegion = (MapSpatialDataRegion)base.PublishClone(context);
			if (m_vectorData != null)
			{
				mapSpatialDataRegion.m_vectorData = (ExpressionInfo)m_vectorData.PublishClone(context);
			}
			return mapSpatialDataRegion;
		}

		internal override void SetExprHostMapMember(MapSpatialDataExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			SetExprHostInternal(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.VectorData, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialDataRegion, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialData, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.VectorData)
				{
					writer.Write(m_vectorData);
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
				if (memberName == MemberName.VectorData)
				{
					m_vectorData = (ExpressionInfo)reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public override ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialDataRegion;
		}

		internal ReportingServices.RdlExpressions.VariantResult EvaluateVectorData(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSpatialDataRegionVectorDataExpression(this, m_map.Name);
		}
	}
}
