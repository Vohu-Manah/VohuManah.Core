using ReportViewer.Common.Renderers;
using ReportingServices.Common;
using ReportingServices.Diagnostics;
using ReportingServices.Interfaces;
using ReportingServices.OnDemandReportRendering;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Resources;
using System.Security.Permissions;

namespace ReportingServices.Rendering.HtmlRenderer
{
	abstract class RenderingExtensionBase : IRenderingExtension, IExtension, ITotalPages
	{
		protected const string m_htmlMimeType = "text/html";

		public abstract string LocalizedName
		{
			get;
		}

		public bool Render(ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			try
			{
				deviceInfo = ((deviceInfo == null) ? new NameValueCollection() : deviceInfo);
				reportServerParameters = ((reportServerParameters == null) ? new NameValueCollection() : reportServerParameters);
				clientCapabilities = ((clientCapabilities == null) ? new NameValueCollection() : clientCapabilities);
				return InternalRender(report, reportServerParameters, deviceInfo, clientCapabilities, ref renderProperties, createAndRegisterStream);
			}
			catch (ReportRenderingException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex2))
				{
					throw;
				}
				throw new ReportRenderingException(ex2, unexpected: true);
			}
		}

		public bool RenderStream(string streamName, ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			try
			{
				deviceInfo = ((deviceInfo == null) ? new NameValueCollection() : deviceInfo);
				reportServerParameters = ((reportServerParameters == null) ? new NameValueCollection() : reportServerParameters);
				clientCapabilities = ((clientCapabilities == null) ? new NameValueCollection() : clientCapabilities);
				return InternalRenderStream(streamName, report, reportServerParameters, deviceInfo, clientCapabilities, ref renderProperties, createAndRegisterStream);
			}
			catch (ReportRenderingException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex2))
				{
					throw;
				}
				throw new ReportRenderingException(ex2, unexpected: true);
			}
		}

		public void GetRenderingResource(CreateAndRegisterStream createAndRegisterStreamCallback, NameValueCollection deviceInfo)
		{
			try
			{
				ROMReport.GetRenderingResource(createAndRegisterStreamCallback, deviceInfo["GetImage"]);
			}
			catch (ReportRenderingException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex2))
				{
					throw;
				}
				throw new ReportRenderingException(ex2, unexpected: true);
			}
		}

		internal static void LoadImageResource(string resourceName, ResourceManager resourceManager, CreateAndRegisterStream createAndRegisterStreamCallback)
		{
			Stream stream = createAndRegisterStreamCallback(resourceName, "gif", null, "image/gif", willSeek: false, StreamOper.CreateAndRegister);
			var adapter = new ImageResourceAdapter();
			adapter.WriteEmbeddedImage(resourceManager, resourceName, stream);
		}

		public virtual void SetConfiguration(string configuration)
		{
		}

		protected abstract bool InternalRender(ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream);

		protected abstract bool InternalRenderStream(string streamName, ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream);
	}
}
