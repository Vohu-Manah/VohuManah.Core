using ReportingServices.ReportProcessing;

namespace ReportingServices.ReportRendering
{
	internal sealed class Line : ReportItem
	{
		public bool Slant => ((ReportingServices.ReportProcessing.Line)base.ReportItemDef).LineSlant;

		internal Line(string uniqueName, int intUniqueName, ReportingServices.ReportProcessing.Line reportItemDef, LineInstance reportItemInstance, RenderingContext renderingContext)
			: base(uniqueName, intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
		}
	}
}
