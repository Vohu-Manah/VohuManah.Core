using System.Drawing;
using Reporting.Rendering;

namespace Reporting.Chart.WebForms.Rendering.Gdi
{
	/// <summary>Milestone A2 adapter — wraps <see cref="System.Drawing.Font"/> behind <see cref="IChartFont"/>.</summary>
	internal sealed class GdiChartFont : IChartFont
	{
		internal Font NativeFont { get; }

		internal GdiChartFont(Font font)
		{
			NativeFont = font;
		}

		public string FontFamilyName => NativeFont.FontFamily.Name;

		public float SizeInPoints => NativeFont.SizeInPoints;

		public FontStyle Style => NativeFont.Style;

		public GraphicsUnit Unit => NativeFont.Unit;

		public void Dispose() => NativeFont.Dispose();
	}
}
