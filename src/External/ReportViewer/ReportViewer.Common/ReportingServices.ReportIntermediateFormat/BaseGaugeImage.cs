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
	internal class BaseGaugeImage : IPersistable
	{
		[NonSerialized]
		protected BaseGaugeImageExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[Reference]
		protected GaugePanel m_gaugePanel;

		private ExpressionInfo m_source;

		private ExpressionInfo m_value;

		private ExpressionInfo m_MIMEType;

		private ExpressionInfo m_transparentColor;

		internal ExpressionInfo Source
		{
			get
			{
				return m_source;
			}
			set
			{
				m_source = value;
			}
		}

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

		internal ExpressionInfo MIMEType
		{
			get
			{
				return m_MIMEType;
			}
			set
			{
				m_MIMEType = value;
			}
		}

		internal ExpressionInfo TransparentColor
		{
			get
			{
				return m_transparentColor;
			}
			set
			{
				m_transparentColor = value;
			}
		}

		internal string OwnerName => m_gaugePanel.Name;

		internal BaseGaugeImageExprHost ExprHost => m_exprHost;

		internal BaseGaugeImage()
		{
		}

		internal BaseGaugeImage(GaugePanel gaugePanel)
		{
			m_gaugePanel = gaugePanel;
		}

		internal virtual void Initialize(InitializationContext context)
		{
			if (m_source != null)
			{
				m_source.Initialize("Source", context);
				context.ExprHostBuilder.BaseGaugeImageSource(m_source);
			}
			if (m_value != null)
			{
				m_value.Initialize("Value", context);
				context.ExprHostBuilder.BaseGaugeImageValue(m_value);
			}
			if (m_MIMEType != null)
			{
				m_MIMEType.Initialize("MIMEType", context);
				context.ExprHostBuilder.BaseGaugeImageMIMEType(m_MIMEType);
			}
			if (m_transparentColor != null)
			{
				m_transparentColor.Initialize("TransparentColor", context);
				context.ExprHostBuilder.BaseGaugeImageTransparentColor(m_transparentColor);
			}
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context)
		{
			BaseGaugeImage baseGaugeImage = (BaseGaugeImage)MemberwiseClone();
			baseGaugeImage.m_gaugePanel = (GaugePanel)context.CurrentDataRegionClone;
			if (m_source != null)
			{
				baseGaugeImage.m_source = (ExpressionInfo)m_source.PublishClone(context);
			}
			if (m_value != null)
			{
				baseGaugeImage.m_value = (ExpressionInfo)m_value.PublishClone(context);
			}
			if (m_MIMEType != null)
			{
				baseGaugeImage.m_MIMEType = (ExpressionInfo)m_MIMEType.PublishClone(context);
			}
			if (m_transparentColor != null)
			{
				baseGaugeImage.m_transparentColor = (ExpressionInfo)m_transparentColor.PublishClone(context);
			}
			return baseGaugeImage;
		}

		internal void SetExprHost(BaseGaugeImageExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Source, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Value, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TransparentColor, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MIMEType, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.GaugePanel, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanel, Token.Reference));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BaseGaugeImage, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.GaugePanel:
					writer.WriteReference(m_gaugePanel);
					break;
				case MemberName.Source:
					writer.Write(m_source);
					break;
				case MemberName.Value:
					writer.Write(m_value);
					break;
				case MemberName.MIMEType:
					writer.Write(m_MIMEType);
					break;
				case MemberName.TransparentColor:
					writer.Write(m_transparentColor);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.GaugePanel:
					m_gaugePanel = reader.ReadReference<GaugePanel>(this);
					break;
				case MemberName.Source:
					m_source = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Value:
					m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MIMEType:
					m_MIMEType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TransparentColor:
					m_transparentColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				MemberName memberName = item.MemberName;
				if (memberName == MemberName.GaugePanel)
				{
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_gaugePanel = (GaugePanel)referenceableItems[item.RefID];
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public virtual ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.BaseGaugeImage;
		}

		internal ReportingServices.OnDemandReportRendering.Image.SourceType EvaluateSource(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateImageSourceType(context.ReportRuntime.EvaluateBaseGaugeImageSourceExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal string EvaluateStringValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context, out bool errorOccurred)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateBaseGaugeImageStringValueExpression(this, m_gaugePanel.Name, out errorOccurred);
		}

		internal byte[] EvaluateBinaryValue(IReportScopeInstance romInstance, OnDemandProcessingContext context, out bool errOccurred)
		{
			context.SetupContext(m_gaugePanel, romInstance);
			return context.ReportRuntime.EvaluateBaseGaugeImageBinaryValueExpression(this, m_gaugePanel.Name, out errOccurred);
		}

		internal string EvaluateMIMEType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateBaseGaugeImageMIMETypeExpression(this, m_gaugePanel.Name);
		}

		internal string EvaluateTransparentColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateBaseGaugeImageTransparentColorExpression(this, m_gaugePanel.Name);
		}
	}
}
