using System.Drawing;
using Reporting.Rendering;

namespace Reporting.Chart.WebForms.Rendering.Skia
{
	/// <summary>
	/// Spike adapter — plain data holder behind <see cref="ITextFormat"/>. No native Skia
	/// object backs this (SkiaSharp's <see cref="SkiaSharp.SKTextAlign"/> is applied directly
	/// in <see cref="SkiaChartGraphics.DrawString(string, IChartFont, IBrush, PointF, ITextFormat)"/>
	/// from these plain properties), matching the "descriptor" shape called out as acceptable
	/// in chart-gdi-type-abstraction.md §3.
	/// </summary>
	internal sealed class SkiaTextFormat : ITextFormat
	{
		public StringAlignment Alignment { get; set; }

		public StringAlignment LineAlignment { get; set; }

		public StringFormatFlags FormatFlags { get; set; }

		public StringTrimming Trimming { get; set; }

		public ITextFormat Clone() => new SkiaTextFormat
		{
			Alignment = Alignment,
			LineAlignment = LineAlignment,
			FormatFlags = FormatFlags,
			Trimming = Trimming
		};

		public void Dispose()
		{
		}
	}
}
