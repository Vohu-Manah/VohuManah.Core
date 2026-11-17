using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Reporting.Rendering;

namespace Reporting.Gauge.WebForms.Rendering
{
	/// <summary>
	/// Gauge-engine counterpart of <c>Reporting.Chart.WebForms.Rendering.IDrawingResourceFactory</c>
	/// (see tasks/gauge-gdi-type-abstraction.md Milestone A1). Constructs the shared, engine-agnostic
	/// resource interfaces from <see cref="Reporting.Rendering"/>. Image abstraction
	/// (<see cref="IChartImage"/>/<see cref="IImageDrawOptions"/>) was added during the GetTextureBrush
	/// prerequisite (Milestone B) via <see cref="WrapImage"/> — see
	/// <see cref="GaugeGraphics.GetTextureBrushResource"/>. Clip-region abstraction
	/// (<see cref="IGaugeClipRegion"/>) was added during Milestone A4 via <see cref="CreateRegion()"/>.
	/// </summary>
	internal interface IGaugeDrawingResourceFactory
	{
		IPen CreatePen(Color color, float width);

		IPen CreatePen(IBrush brush, float width);

		ISolidBrush CreateSolidBrush(Color color);

		ILinearGradientBrush CreateLinearGradientBrush(RectangleF rect, Color startColor, Color endColor, float angle);

		/// <summary>
		/// GDI+'s point-pair <c>LinearGradientBrush(PointF, PointF, Color, Color)</c> constructor,
		/// needed by <c>XamlRenderer.CreateBrush</c>'s absolute/relative gradient-stop geometry
		/// (see tasks/gauge-gdi-type-abstraction.md item 2).
		/// </summary>
		ILinearGradientBrush CreateLinearGradientBrush(PointF point1, PointF point2, Color color1, Color color2);

		ITextureBrush CreateTextureBrush(IChartImage image, WrapMode wrapMode);

		/// <summary>
		/// Construct a texture brush drawn into <paramref name="rect"/> with colour-key/wrap-mode
		/// <paramref name="options"/> applied — GDI+'s <c>TextureBrush(Image, RectangleF, ImageAttributes)</c>
		/// constructor, needed by <see cref="GaugeGraphics.GetTextureBrushResource"/>.
		/// </summary>
		ITextureBrush CreateTextureBrush(IChartImage image, RectangleF rect, IImageDrawOptions options);

		IHatchBrush CreateHatchBrush(HatchStyle style, Color foreColor, Color backColor);

		IPathGradientBrush CreatePathGradientBrush(IGraphicsPath path);

		IChartFont CreateFont(string familyName, float sizeInPoints);

		IChartFont CreateFont(string familyName, float size, FontStyle style);

		IChartFont CreateFont(string familyName, float size, FontStyle style, GraphicsUnit unit);

		/// <summary>Wrap an already-constructed native <see cref="Font"/>, without reconstructing it from decomposed properties (see the Chart engine's identically-named method for why this matters).</summary>
		IChartFont WrapFont(Font font);

		ITextFormat CreateTextFormat();

		ITextFormat CreateTypographicTextFormat();

		IGraphicsPath CreatePath();

		IGraphicsPath CreatePath(PointF[] points, byte[] types);

		/// <summary>
		/// Wrap an already-constructed native <see cref="GraphicsPath"/> as an <see cref="IGraphicsPath"/> —
		/// a bridge for legacy concrete-path-building code (e.g. <c>BackFrame.GetFramePath</c>) that stays
		/// GDI+-only/concrete for now (found during the clip-region prerequisite; see
		/// tasks/gauge-gdi-type-abstraction.md Milestone A4). Mirrors <see cref="WrapImage"/>'s role.
		/// </summary>
		IGraphicsPath WrapPath(GraphicsPath path);

		/// <summary>
		/// Reverse of <see cref="WrapPath"/> — unwraps an interface-typed path back to its native
		/// <see cref="GraphicsPath"/>, for bridging into still-concrete-only consumers. <c>HotRegionList</c>
		/// now has an <see cref="IGraphicsPath"/>-typed <c>SetHotRegion</c> overload (item 1 —
		/// see tasks/gauge-gdi-type-abstraction.md) that bridges internally via <c>PathPoints</c>/<c>PathTypes</c>,
		/// so callers holding an interface-typed path no longer need to unwrap it themselves for that call.
		/// </summary>
		GraphicsPath UnwrapPath(IGraphicsPath path);

		/// <summary>
		/// Wrap an already-loaded native image as an <see cref="IChartImage"/> — a bridge for
		/// <c>common.ImageLoader</c>'s legacy loading pipeline, which remains GDI+-only/concrete
		/// (found during the GetTextureBrush prerequisite; see tasks/gauge-gdi-type-abstraction.md
		/// Milestone B). Not available on backends that can't construct <see cref="Image"/> at all.
		/// </summary>
		IChartImage WrapImage(Image image);

		IImageDrawOptions CreateImageDrawOptions();

		IGaugeClipRegion CreateRegion();

		IGaugeClipRegion CreateRegion(RectangleF rect);

		IGaugeClipRegion CreateRegion(IGraphicsPath path);
	}
}
