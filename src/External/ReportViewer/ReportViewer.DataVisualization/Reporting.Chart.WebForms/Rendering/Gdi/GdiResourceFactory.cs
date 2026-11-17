using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using Reporting.Rendering;

namespace Reporting.Chart.WebForms.Rendering.Gdi
{
	/// <summary>
	/// Milestone A2 — the GDI+ implementation of <see cref="IDrawingResourceFactory"/>.
	/// Behavior-identical to today: every method constructs the same concrete
	/// <c>System.Drawing</c> object the engine currently creates directly, just
	/// wrapped behind its port interface.
	/// </summary>
	internal sealed class GdiResourceFactory : IDrawingResourceFactory
	{
		public IPen CreatePen(Color color, float width) => new GdiPen(color, width);

		public IPen CreatePen(IBrush brush, float width) => new GdiPen(NativeBrush(brush), width);

		private static Brush NativeBrush(IBrush brush) => brush switch
		{
			GdiSolidBrush b => b.NativeBrush,
			GdiLinearGradientBrush b => b.NativeBrush,
			GdiTextureBrush b => b.NativeBrush,
			GdiHatchBrush b => b.NativeBrush,
			GdiPathGradientBrush b => b.NativeBrush,
			_ => throw new NotSupportedException($"Unrecognized IBrush implementation: {brush.GetType()}"),
		};

		public ISolidBrush CreateSolidBrush(Color color) => new GdiSolidBrush(color);

		public ILinearGradientBrush CreateLinearGradientBrush(RectangleF rect, Color startColor, Color endColor, float angle) =>
			new GdiLinearGradientBrush(rect, startColor, endColor, angle);

		public ILinearGradientBrush CreateLinearGradientBrush(PointF point1, PointF point2, Color color1, Color color2) =>
			new GdiLinearGradientBrush(point1, point2, color1, color2);

		public ITextureBrush CreateTextureBrush(IChartImage image, WrapMode wrapMode) =>
			new GdiTextureBrush(((GdiChartImage)image).NativeImage, wrapMode);

		public ITextureBrush CreateTextureBrush(IChartImage image, RectangleF rect, IImageDrawOptions options) =>
			new GdiTextureBrush(((GdiChartImage)image).NativeImage, rect, ((GdiImageDrawOptions)options)?.NativeAttributes);

		public IHatchBrush CreateHatchBrush(HatchStyle style, Color foreColor, Color backColor) =>
			new GdiHatchBrush(style, foreColor, backColor);

		public IPathGradientBrush CreatePathGradientBrush(IGraphicsPath path) => new GdiPathGradientBrush(path);

		public IChartImage LoadImage(Stream stream) => new GdiChartImage(Image.FromStream(stream));

		public IChartImage WrapImage(Image image) => new GdiChartImage(image);

		public IChartFont CreateFont(string familyName, float sizeInPoints) =>
			new GdiChartFont(new Font(familyName, sizeInPoints));

		public IChartFont CreateFont(string familyName, float size, FontStyle style) =>
			new GdiChartFont(new Font(familyName, size, style));

		public IChartFont CreateFont(string familyName, float size, FontStyle style, GraphicsUnit unit) =>
			new GdiChartFont(new Font(familyName, size, style, unit));

		public IChartFont DeriveFont(IChartFont prototype, FontStyle style)
		{
			var nativePrototype = ((GdiChartFont)prototype).NativeFont;
			return new GdiChartFont(new Font(nativePrototype.FontFamily, nativePrototype.Size, style, nativePrototype.Unit));
		}

		public IChartFont DeriveFont(IChartFont prototype, float newSizeInPoints)
		{
			var nativePrototype = ((GdiChartFont)prototype).NativeFont;
			return new GdiChartFont(new Font(nativePrototype.FontFamily, newSizeInPoints, nativePrototype.Style, GraphicsUnit.Point));
		}

		public IChartFont WrapFont(Font font) => new GdiChartFont(font);

		public ITextFormat CreateTextFormat() => new GdiTextFormat(new StringFormat());

		public ITextFormat CreateTypographicTextFormat() => new GdiTextFormat(new StringFormat(StringFormat.GenericTypographic));

		public ITextFormat CreateDefaultTextFormat() => new GdiTextFormat(new StringFormat(StringFormat.GenericDefault));

		public IGraphicsPath CreatePath() => new GdiGraphicsPath();

		public IGraphicsPath CreatePath(PointF[] points, byte[] types) => new GdiGraphicsPath(points, types);

		public IClipRegion CreateRegion() => new GdiClipRegion();

		public IClipRegion CreateRegion(RectangleF rect) => new GdiClipRegion(rect);

		public IClipRegion CreateRegion(IGraphicsPath path) => new GdiClipRegion(path);

		public IImageDrawOptions CreateImageDrawOptions() => new GdiImageDrawOptions();
	}
}
