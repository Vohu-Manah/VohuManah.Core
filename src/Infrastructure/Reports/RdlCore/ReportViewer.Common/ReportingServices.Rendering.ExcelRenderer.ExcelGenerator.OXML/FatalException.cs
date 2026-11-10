using System;

namespace ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML
{
	internal class FatalException : Exception
	{
		public FatalException()
			: base(ExcelRenderRes.ArgumentInvalid)
		{
		}
	}
}
