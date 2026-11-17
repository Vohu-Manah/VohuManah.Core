using System.Drawing;
using System.Numerics;
using SkiaSharp;
using Reporting.Rendering;

namespace Reporting.Chart.WebForms.Rendering.Skia
{
	/// <summary>
	/// Real adapter (E1) behind <see cref="IClipRegion"/> — backs region algebra with
	/// <see cref="SKPath"/> boolean operations (<see cref="SKPath.Op(SKPath, SKPathOp)"/>).
	/// <see cref="SKPath"/> can only represent bounded geometry, so unlike GDI+'s
	/// <see cref="Region"/> (which can represent a true infinite plane, or its complement),
	/// "infinite" here is approximated by a very large bounding rectangle — <see cref="InfiniteBounds"/>,
	/// chosen far larger than any real chart surface. This is exact for every operation actually
	/// used by the chart engine (Intersect/Union/Exclude/Xor/Complement against real, finite chart
	/// geometry) and only diverges from true GDI+ semantics in the degenerate case of composing
	/// multiple infinite regions with each other, which the engine never does.
	/// <see cref="SkiaChartGraphics.GetClipRegion"/>/<see cref="SkiaChartGraphics.SetClipRegion"/>
	/// still don't wire this into the canvas's own clip stack (that needs Save/Restore parity too,
	/// itself still a GDI+-typed-only stub) — this class is a correct, real building block for that
	/// still-open follow-up, not the follow-up itself.
	/// </summary>
	internal sealed class SkiaClipRegion : IClipRegion
	{
		/// <summary>Stand-in for "infinite" — see class remarks. Large enough that no real chart geometry approaches it.</summary>
		internal static readonly RectangleF InfiniteBounds = new RectangleF(-1_000_000f, -1_000_000f, 2_000_000f, 2_000_000f);

		private SKPath path;
		private bool isInfinite;

		internal SkiaClipRegion()
		{
			isInfinite = true;
		}

		internal SkiaClipRegion(RectangleF rect)
		{
			path = new SKPath();
			path.AddRect(SkiaConvert.ToSKRect(rect));
		}

		internal SkiaClipRegion(IGraphicsPath sourcePath)
		{
			path = new SKPath(((SkiaGraphicsPath)sourcePath).NativePath);
		}

		private SkiaClipRegion(SKPath path, bool isInfinite)
		{
			this.path = path;
			this.isInfinite = isInfinite;
		}

		private SKPath EffectivePath()
		{
			if (isInfinite)
			{
				var infinite = new SKPath();
				infinite.AddRect(SkiaConvert.ToSKRect(InfiniteBounds));
				return infinite;
			}
			return path;
		}

		private void Combine(SKPath other, SKPathOp op)
		{
			using SKPath basePath = EffectivePath();
			SKPath result = basePath.Op(other, op) ?? new SKPath();
			path?.Dispose();
			if (isInfinite)
			{
				basePath.Dispose();
			}
			path = result;
			isInfinite = false;
		}

		private void Combine(RectangleF rect, SKPathOp op)
		{
			using var rectPath = new SKPath();
			rectPath.AddRect(SkiaConvert.ToSKRect(rect));
			Combine(rectPath, op);
		}

		public void Intersect(RectangleF rect) => Combine(rect, SKPathOp.Intersect);

		public void Intersect(IGraphicsPath path) => Combine(((SkiaGraphicsPath)path).NativePath, SKPathOp.Intersect);

		public void Union(RectangleF rect) => Combine(rect, SKPathOp.Union);

		public void Exclude(RectangleF rect) => Combine(rect, SKPathOp.Difference);

		public void Complement(IGraphicsPath path) => Combine(((SkiaGraphicsPath)path).NativePath, SKPathOp.ReverseDifference);

		public void Xor(RectangleF rect) => Combine(rect, SKPathOp.Xor);

		public void MakeEmpty()
		{
			path?.Dispose();
			path = new SKPath();
			isInfinite = false;
		}

		public void MakeInfinite()
		{
			path?.Dispose();
			path = null;
			isInfinite = true;
		}

		public bool IsVisible(PointF point) => isInfinite || path.Contains(point.X, point.Y);

		public void Transform(Matrix3x2 matrix)
		{
			if (isInfinite)
			{
				return;
			}
			path.Transform(new SKMatrix(matrix.M11, matrix.M21, matrix.M31, matrix.M12, matrix.M22, matrix.M32, 0, 0, 1));
		}

		public void Translate(float dx, float dy)
		{
			if (isInfinite)
			{
				return;
			}
			path.Transform(SKMatrix.CreateTranslation(dx, dy));
		}

		public IClipRegion Clone() => isInfinite
			? new SkiaClipRegion(null, isInfinite: true)
			: new SkiaClipRegion(new SKPath(path), isInfinite: false);

		public RectangleF GetBounds(IChartRenderingEngine engine) => isInfinite ? InfiniteBounds : ToRectangleF(path.Bounds);

		public bool IsEmpty(IChartRenderingEngine engine) => !isInfinite && path.IsEmpty;

		public bool IsInfinite(IChartRenderingEngine engine) => isInfinite;

		public void Dispose() => path?.Dispose();

		/// <summary>Caller-owned copy of the region's current geometry (the sentinel rect if infinite), for drawing (e.g. <see cref="SkiaChartGraphics.FillRegion"/>) — never the live field, so the caller's disposal can't corrupt this region.</summary>
		internal SKPath ToDrawablePath() => isInfinite ? EffectivePath() : new SKPath(path);

		private static RectangleF ToRectangleF(SKRect rect) => new RectangleF(rect.Left, rect.Top, rect.Width, rect.Height);
	}
}
