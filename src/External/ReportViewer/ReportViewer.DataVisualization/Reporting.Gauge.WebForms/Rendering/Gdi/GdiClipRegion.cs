using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using Reporting.Rendering;

namespace Reporting.Gauge.WebForms.Rendering.Gdi
{
	/// <summary>
	/// Milestone A4 adapter — wraps <see cref="System.Drawing.Region"/> behind <see cref="IGaugeClipRegion"/>.
	/// Gauge-owned, separate from the Chart engine's identically-shaped <c>GdiClipRegion</c>, per the
	/// migration's decoupled-adapter design (see tasks/gauge-gdi-type-abstraction.md).
	/// </summary>
	internal sealed class GdiClipRegion : IGaugeClipRegion
	{
		internal Region NativeRegion { get; }

		internal GdiClipRegion()
		{
			NativeRegion = new Region();
		}

		internal GdiClipRegion(RectangleF rect)
		{
			NativeRegion = new Region(rect);
		}

		internal GdiClipRegion(IGraphicsPath path)
		{
			NativeRegion = new Region(((GdiGraphicsPath)path).NativePath);
		}

		/// <summary>Wraps an existing native <see cref="Region"/> (e.g. the current <c>Graphics.Clip</c>) rather than creating a new one.</summary>
		internal GdiClipRegion(Region existingRegion)
		{
			NativeRegion = existingRegion;
		}

		public void Intersect(RectangleF rect) => NativeRegion.Intersect(rect);

		public void Intersect(IGraphicsPath path) => NativeRegion.Intersect(((GdiGraphicsPath)path).NativePath);

		public void Union(RectangleF rect) => NativeRegion.Union(rect);

		public void Exclude(RectangleF rect) => NativeRegion.Exclude(rect);

		public void Complement(IGraphicsPath path) => NativeRegion.Complement(((GdiGraphicsPath)path).NativePath);

		public void Xor(RectangleF rect) => NativeRegion.Xor(rect);

		public void MakeEmpty() => NativeRegion.MakeEmpty();

		public void MakeInfinite() => NativeRegion.MakeInfinite();

		public bool IsVisible(PointF point) => NativeRegion.IsVisible(point);

		public void Transform(Matrix3x2 matrix)
		{
			using Matrix nativeMatrix = new Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.M31, matrix.M32);
			NativeRegion.Transform(nativeMatrix);
		}

		public void Translate(float dx, float dy) => NativeRegion.Translate(dx, dy);

		public IGaugeClipRegion Clone() => new GdiClipRegion(NativeRegion.Clone());

		public RectangleF GetBounds(IGaugeRenderingEngine engine) => NativeRegion.GetBounds(engine.Graphics);

		public bool IsEmpty(IGaugeRenderingEngine engine) => NativeRegion.IsEmpty(engine.Graphics);

		public bool IsInfinite(IGaugeRenderingEngine engine) => NativeRegion.IsInfinite(engine.Graphics);

		public void Dispose() => NativeRegion.Dispose();
	}
}
