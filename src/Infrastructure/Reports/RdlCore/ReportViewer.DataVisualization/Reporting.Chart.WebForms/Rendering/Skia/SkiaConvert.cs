using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using SkiaSharp;
using Reporting.Rendering;

namespace Reporting.Chart.WebForms.Rendering.Skia
{
	/// <summary>
	/// Spike helper (tasks/chart-cross-platform-implementation.md Phase 0) — the small set
	/// of value-type conversions every Skia adapter needs. <see cref="Color"/>/<see cref="PointF"/>/
	/// <see cref="RectangleF"/> are the portable <c>System.Drawing.Primitives</c> value types
	/// (see chart-gdi-type-abstraction.md §2.1) — converting them does not touch GDI+.
	/// </summary>
	internal static class SkiaConvert
	{
		internal static SKColor ToSKColor(Color color) => new SKColor(color.R, color.G, color.B, color.A);

		internal static Color ToColor(SKColor color) => Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);

		internal static SKPoint ToSKPoint(PointF point) => new SKPoint(point.X, point.Y);

		internal static SKRect ToSKRect(RectangleF rect) => new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);

		internal static SKStrokeCap ToSKStrokeCap(LineCap cap) => cap switch
		{
			LineCap.Round or LineCap.RoundAnchor => SKStrokeCap.Round,
			LineCap.Square or LineCap.SquareAnchor => SKStrokeCap.Square,
			_ => SKStrokeCap.Butt,
		};

		internal static SKStrokeJoin ToSKStrokeJoin(LineJoin join) => join switch
		{
			LineJoin.Round => SKStrokeJoin.Round,
			LineJoin.Bevel => SKStrokeJoin.Bevel,
			_ => SKStrokeJoin.Miter,
		};

		/// <summary>Bridges a brush's <c>Matrix3x2</c> transform (see <c>IBrush.MultiplyTransform</c>) into an <see cref="SKMatrix"/>.</summary>
		internal static SKMatrix ToSKMatrix(Matrix3x2 m) => new SKMatrix(m.M11, m.M21, m.M31, m.M12, m.M22, m.M32, 0f, 0f, 1f);
	}
}
