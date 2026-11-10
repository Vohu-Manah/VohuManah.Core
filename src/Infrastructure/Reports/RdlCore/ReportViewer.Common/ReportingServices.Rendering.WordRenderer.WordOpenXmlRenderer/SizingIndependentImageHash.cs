using ReportingServices.Rendering.RPLProcessing;

namespace ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer
{
	internal sealed class SizingIndependentImageHash : ImageHash
	{
		public SizingIndependentImageHash(byte[] md4)
			: base(md4, RPLFormat.Sizings.AutoSize, 0, 0)
		{
		}
	}
}
