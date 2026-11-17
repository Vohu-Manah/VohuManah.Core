using ReportingServices.Diagnostics;
using ReportingServices.Interfaces;
using ReportingServices.OnDemandReportRendering;
using System.Collections;
using System.Collections.Specialized;

namespace ReportingServices.ReportProcessing.Execution
{
	internal class RenderReportOdpSnapshotStream : RenderReportOdpSnapshot
	{
		private readonly string m_streamName;

		protected override bool IsRenderStream => true;

		public RenderReportOdpSnapshotStream(ProcessingContext pc, RenderingContext rc, IConfiguration configuration, string streamName)
			: base(pc, rc, configuration)
		{
			m_streamName = streamName;
		}

		protected override bool InvokeRenderer(IRenderingExtension renderer, ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			return renderer.RenderStream(m_streamName, report, reportServerParameters, deviceInfo, clientCapabilities, ref renderProperties, createAndRegisterStream);
		}
	}
}
