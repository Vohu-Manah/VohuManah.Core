using SkiaSharp;
using Reporting.Rendering;

namespace Reporting.Chart.WebForms.Rendering.Skia
{
	/// <summary>Wraps a decoded <see cref="SKBitmap"/> behind <see cref="IChartImage"/> — the Skia counterpart to <c>GdiChartImage</c>.</summary>
	internal sealed class SkiaChartImage : IChartImage
	{
		internal SKBitmap NativeBitmap { get; }

		internal SkiaChartImage(SKBitmap bitmap)
		{
			NativeBitmap = bitmap;
		}

		public int Width => NativeBitmap.Width;

		public int Height => NativeBitmap.Height;

		public void Dispose() => NativeBitmap.Dispose();
	}
}
