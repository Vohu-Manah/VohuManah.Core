using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.RdlExpressions;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportProcessing.ReportObjectModel;

namespace ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal abstract class ReportItemImpl : ReportingServices.ReportProcessing.ReportObjectModel.ReportItem
	{
		internal ReportingServices.ReportIntermediateFormat.ReportItem m_item;

		internal ReportingServices.RdlExpressions.ReportRuntime m_reportRT;

		internal IErrorContext m_iErrorContext;

		internal IScope m_scope;

		internal string Name => m_item.Name;

		internal IScope Scope
		{
			set
			{
				m_scope = value;
			}
		}

		internal ReportItemImpl(ReportingServices.ReportIntermediateFormat.ReportItem itemDef, ReportingServices.RdlExpressions.ReportRuntime reportRT, IErrorContext iErrorContext)
		{
			Global.Tracer.Assert(itemDef != null, "(null != itemDef)");
			Global.Tracer.Assert(reportRT != null, "(null != reportRT)");
			Global.Tracer.Assert(iErrorContext != null, "(null != iErrorContext)");
			m_item = itemDef;
			m_reportRT = reportRT;
			m_iErrorContext = iErrorContext;
		}

		internal abstract void Reset();

		internal abstract void Reset(ReportingServices.RdlExpressions.VariantResult aResult);
	}
}
