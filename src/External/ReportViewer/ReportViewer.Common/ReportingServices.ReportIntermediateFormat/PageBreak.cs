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
	internal class PageBreak : IPersistable
	{
		private PageBreakLocation m_pageBreakLocation;

		private ExpressionInfo m_disabled;

		private ExpressionInfo m_resetPageNumber;

		[NonSerialized]
		private PageBreakExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal PageBreakLocation BreakLocation
		{
			get
			{
				return m_pageBreakLocation;
			}
			set
			{
				m_pageBreakLocation = value;
			}
		}

		internal ExpressionInfo ResetPageNumber
		{
			get
			{
				return m_resetPageNumber;
			}
			set
			{
				m_resetPageNumber = value;
			}
		}

		internal ExpressionInfo Disabled
		{
			get
			{
				return m_disabled;
			}
			set
			{
				m_disabled = value;
			}
		}

		internal PageBreakExprHost ExprHost
		{
			get
			{
				return m_exprHost;
			}
			set
			{
				m_exprHost = value;
			}
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			PageBreak pageBreak = (PageBreak)MemberwiseClone();
			if (m_disabled != null)
			{
				pageBreak.m_disabled = (ExpressionInfo)m_disabled.PublishClone(context);
			}
			if (m_resetPageNumber != null)
			{
				pageBreak.m_resetPageNumber = (ExpressionInfo)m_resetPageNumber.PublishClone(context);
			}
			return pageBreak;
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.PageBreakStart();
			if (m_disabled != null)
			{
				m_disabled.Initialize("Disabled", context);
				context.ExprHostBuilder.Disabled(m_disabled);
			}
			if (m_resetPageNumber != null)
			{
				m_resetPageNumber.Initialize("ResetPageNumber", context);
				context.ExprHostBuilder.ResetPageNumber(m_resetPageNumber);
			}
			context.ExprHostBuilder.PageBreakEnd();
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.PageBreakLocation:
					writer.WriteEnum((int)m_pageBreakLocation);
					break;
				case MemberName.Disabled:
					writer.Write(m_disabled);
					break;
				case MemberName.ResetPageNumber:
					writer.Write(m_resetPageNumber);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.PageBreakLocation:
					m_pageBreakLocation = (PageBreakLocation)reader.ReadEnum();
					break;
				case MemberName.Disabled:
					m_disabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ResetPageNumber:
					m_resetPageNumber = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false, "No references to resolve");
		}

		public ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageBreak;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.PageBreakLocation, Token.Enum));
			list.Add(new MemberInfo(MemberName.Disabled, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ResetPageNumber, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageBreak, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		internal void SetExprHost(PageBreakExprHost pageBreakExpressionHost, ObjectModelImpl reportObjectModel)
		{
			if (pageBreakExpressionHost != null)
			{
				m_exprHost = pageBreakExpressionHost;
				Global.Tracer.Assert(m_exprHost != null && reportObjectModel != null);
				m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal bool EvaluateDisabled(IReportScopeInstance romInstance, OnDemandProcessingContext context, IPageBreakOwner pageBreakOwner)
		{
			context.SetupContext(pageBreakOwner.InstancePath, romInstance);
			return context.ReportRuntime.EvaluatePageBreakDisabledExpression(this, m_disabled, pageBreakOwner.ObjectType, pageBreakOwner.ObjectName);
		}

		internal bool EvaluateResetPageNumber(IReportScopeInstance romInstance, OnDemandProcessingContext context, IPageBreakOwner pageBreakOwner)
		{
			context.SetupContext(pageBreakOwner.InstancePath, romInstance);
			return context.ReportRuntime.EvaluatePageBreakResetPageNumberExpression(this, m_resetPageNumber, pageBreakOwner.ObjectType, pageBreakOwner.ObjectName);
		}
	}
}
