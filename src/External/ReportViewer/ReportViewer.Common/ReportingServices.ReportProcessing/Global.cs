using ReportingServices.Diagnostics.Utilities;
using System.Reflection;

namespace ReportingServices.ReportProcessing
{
	internal sealed class Global
	{
		internal static readonly string ReportProcessingNamespace = "ReportingServices.ReportProcessing";

		internal static RSTrace Tracer = RSTrace.ProcessingTracer;

		internal static RSTrace RenderingTracer = RSTrace.RenderingTracer;

		internal static string ReportProcessingLocation = Assembly.GetExecutingAssembly().Location;

		private Global()
		{
		}
	}
}
