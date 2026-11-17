using ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser;

namespace ReportingServices.Rendering.ExcelOpenXmlRenderer.Models
{
	internal interface IOoxmlCtWrapperModel
	{
		OoxmlComplexType OoxmlTag
		{
			get;
		}

		void Cleanup();
	}
}
