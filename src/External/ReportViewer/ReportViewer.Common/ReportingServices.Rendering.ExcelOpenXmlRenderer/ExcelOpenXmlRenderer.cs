using ReportingServices.Interfaces;
using ReportingServices.Rendering.ExcelRenderer;
using ReportingServices.Rendering.ExcelRenderer.Excel;
using ReportingServices.Rendering.ExcelRenderer.ExcelGenerator;
using System.IO;
using System.Security.Permissions;

namespace ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal class ExcelOpenXmlRenderer : ReportingServices.Rendering.ExcelRenderer.ExcelRenderer
	{
		public override string LocalizedName => ExcelRenderRes.ExcelOoxmlLocalizedName;

		protected override Stream CreateFinalOutputStream(string name, CreateAndRegisterStream createAndRegisterStream)
		{
			return createAndRegisterStream(name, "xlsx", null, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", willSeek: false, StreamOper.CreateAndRegister);
		}

		internal override IExcelGenerator CreateExcelGenerator(ExcelGeneratorConstants.CreateTempStream createTempStream)
		{
			return new OpenXmlGenerator(createTempStream);
		}
	}
}
