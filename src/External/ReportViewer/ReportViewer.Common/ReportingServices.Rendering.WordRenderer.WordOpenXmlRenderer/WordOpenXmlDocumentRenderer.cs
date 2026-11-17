using ReportingServices.Interfaces;
using ReportingServices.Rendering.SPBProcessing;

namespace ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer
{
	internal class WordOpenXmlDocumentRenderer : WordDocumentRendererBase
	{
		public override string LocalizedName => WordRenderRes.WordOpenXmlLocalizedName;

		internal override IWordWriter NewWordWriter()
		{
			return new WordOpenXmlWriter();
		}

		protected override WordRenderer NewWordRenderer(CreateAndRegisterStream createAndRegisterStream, DeviceInfo deviceInfoObj, ReportingServices.Rendering.SPBProcessing.SPBProcessing spbProcessing, IWordWriter writer, string reportName)
		{
			return new WordOpenXmlRenderer(createAndRegisterStream, spbProcessing, writer, deviceInfoObj, reportName);
		}
	}
}
