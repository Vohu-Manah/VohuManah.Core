using System.Drawing;
using System.Drawing.Drawing2D;
using Reporting.Rendering;

namespace Reporting.Map.WebForms.Rendering
{
	/// <summary>
	/// Map engine's own drawing-resource factory (Milestone A — see
	/// tasks/map-engine-cross-platform.md). Mirrors Chart's
	/// <c>IDrawingResourceFactory</c> shape, but is a separate interface/adapter
	/// per the established "Gauge's adapters are separate implementations from
	/// Chart's identically-shaped ones by design" convention (docs/rendering-abstractions.md).
	/// Scoped to the resource kinds Map's dual-overload rendering-engine methods
	/// actually need for this milestone (Pen, Solid/LinearGradient/Hatch/PathGradient
	/// brushes, Font, TextFormat, GraphicsPath) — Texture brushes, images, and clip
	/// regions are deliberately deferred to a later increment.
	/// </summary>
	internal interface IMapDrawingResourceFactory
	{
		IPen CreatePen(Color color, float width);

		IPen CreatePen(IBrush brush, float width);

		ISolidBrush CreateSolidBrush(Color color);

		ILinearGradientBrush CreateLinearGradientBrush(RectangleF rect, Color startColor, Color endColor, float angle);

		ILinearGradientBrush CreateLinearGradientBrush(PointF point1, PointF point2, Color color1, Color color2);

		IHatchBrush CreateHatchBrush(HatchStyle style, Color foreColor, Color backColor);

		IPathGradientBrush CreatePathGradientBrush(IGraphicsPath path);

		IChartFont CreateFont(string familyName, float sizeInPoints);

		IChartFont CreateFont(string familyName, float size, FontStyle style);

		IChartFont CreateFont(string familyName, float size, FontStyle style, GraphicsUnit unit);

		IChartFont WrapFont(Font font);

		ITextFormat CreateTextFormat();

		ITextFormat CreateTypographicTextFormat();

		IGraphicsPath CreatePath();

		IGraphicsPath CreatePath(PointF[] points, byte[] types);

		IGraphicsPath WrapPath(GraphicsPath path);

		/// <summary>Unwrap an <see cref="IGraphicsPath"/> back to a native <see cref="GraphicsPath"/> — needed for <c>HotRegionList.SetHotRegion</c>, whose interactive hit-testing (<c>GraphicsPath.IsOutlineVisible(float,float,Pen)</c>) has no interface equivalent and is a permanent, concrete-only boundary (same shape as Gauge's own <c>HotRegionList.SetHotRegion</c>/<c>UnwrapPath</c>). Only meaningful while Gdi is the sole Map backend.</summary>
		GraphicsPath UnwrapPath(IGraphicsPath path);

		/// <summary>Wrap an already-constructed native <see cref="Pen"/> as an <see cref="IPen"/> — needed for MapGraphics helper methods (e.g. shadow/marker pens) that build a concrete Pen internally.</summary>
		IPen WrapPen(Pen pen);

		/// <summary>Wrap an already-constructed native <see cref="Brush"/> (Solid/LinearGradient/Hatch/PathGradient) as an <see cref="IBrush"/> — needed for MapGraphics helper methods (e.g. <c>GetShadowBrush</c>/<c>CreateBrush</c>) that build a concrete Brush internally. Texture brushes are not supported (Milestone A scope).</summary>
		IBrush WrapBrush(Brush brush);
	}
}
