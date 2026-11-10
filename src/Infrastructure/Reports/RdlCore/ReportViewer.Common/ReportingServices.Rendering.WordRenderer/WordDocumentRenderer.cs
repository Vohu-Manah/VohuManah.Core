using ReportingServices.Interfaces;
using ReportingServices.Rendering.SPBProcessing;

namespace ReportingServices.Rendering.WordRenderer
{
	internal class WordDocumentRenderer : WordDocumentRendererBase
	{
		internal override IWordWriter NewWordWriter()
		{
			return new Word97Writer();
		}

		protected override WordRenderer NewWordRenderer(CreateAndRegisterStream createAndRegisterStream, DeviceInfo deviceInfoObj, ReportingServices.Rendering.SPBProcessing.SPBProcessing spbProcessing, IWordWriter writer, string reportName)
		{
			return new Word97Renderer(createAndRegisterStream, spbProcessing, writer, deviceInfoObj, reportName);
		}
	}
}
