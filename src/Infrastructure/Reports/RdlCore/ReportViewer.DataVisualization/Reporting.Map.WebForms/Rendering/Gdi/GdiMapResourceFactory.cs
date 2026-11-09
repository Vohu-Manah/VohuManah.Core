using System.Drawing;
using System.Drawing.Drawing2D;
using Reporting.Rendering;

namespace Reporting.Map.WebForms.Rendering.Gdi
{
	/// <summary>Map engine Milestone A adapter — implements <see cref="IMapDrawingResourceFactory"/> over GDI+.</summary>
	internal sealed class GdiMapResourceFactory : IMapDrawingResourceFactory
	{
		public IPen CreatePen(Color color, float width) => new GdiPen(color, width);

		public IPen CreatePen(IBrush brush, float width) => new GdiPen(NativeBrush(brush), width);

		public ISolidBrush CreateSolidBrush(Color color) => new GdiSolidBrush(color);

		public ILinearGradientBrush CreateLinearGradientBrush(RectangleF rect, Color startColor, Color endColor, float angle) =>
			new GdiLinearGradientBrush(rect, startColor, endColor, angle);

		public ILinearGradientBrush CreateLinearGradientBrush(PointF point1, PointF point2, Color color1, Color color2) =>
			new GdiLinearGradientBrush(point1, point2, color1, color2);

		public IHatchBrush CreateHatchBrush(HatchStyle style, Color foreColor, Color backColor) => new GdiHatchBrush(style, foreColor, backColor);

		public IPathGradientBrush CreatePathGradientBrush(IGraphicsPath path) => new GdiPathGradientBrush(path);

		public IChartFont CreateFont(string familyName, float sizeInPoints) => new GdiChartFont(new Font(familyName, sizeInPoints));

		public IChartFont CreateFont(string familyName, float size, FontStyle style) => new GdiChartFont(new Font(familyName, size, style));

		public IChartFont CreateFont(string familyName, float size, FontStyle style, GraphicsUnit unit) => new GdiChartFont(new Font(familyName, size, style, unit));

		public IChartFont WrapFont(Font font) => new GdiChartFont(font);

		public ITextFormat CreateTextFormat() => new GdiTextFormat(new StringFormat());

		public ITextFormat CreateTypographicTextFormat() => new GdiTextFormat(new StringFormat(StringFormat.GenericTypographic));

		public IGraphicsPath CreatePath() => new GdiGraphicsPath();

		public IGraphicsPath CreatePath(PointF[] points, byte[] types) => new GdiGraphicsPath(points, types);

		public IGraphicsPath WrapPath(GraphicsPath path) => new GdiGraphicsPath(path);

		public GraphicsPath UnwrapPath(IGraphicsPath path) => ((GdiGraphicsPath)path).NativePath;

		public IPen WrapPen(Pen pen) => new GdiPen(pen);

		public IBrush WrapBrush(Brush brush) => brush switch
		{
			SolidBrush b => new GdiSolidBrush(b),
			LinearGradientBrush b => new GdiLinearGradientBrush(b),
			HatchBrush b => new GdiHatchBrush(b),
			PathGradientBrush b => new GdiPathGradientBrush(b),
			_ => throw new System.NotSupportedException($"Unrecognized Brush implementation: {brush.GetType()}"),
		};

		private static Brush NativeBrush(IBrush brush) => brush switch
		{
			GdiSolidBrush b => b.NativeBrush,
			GdiLinearGradientBrush b => b.NativeBrush,
			GdiHatchBrush b => b.NativeBrush,
			GdiPathGradientBrush b => b.NativeBrush,
			_ => throw new System.NotSupportedException($"Unrecognized IBrush implementation: {brush.GetType()}"),
		};
	}
}
