using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using SkiaSharp;
using Reporting.Rendering;

namespace Reporting.Chart.WebForms.Rendering.Skia
{
	/// <summary>Spike adapter — wraps a fill-styled <see cref="SKPaint"/> behind <see cref="ISolidBrush"/>.</summary>
	internal sealed class SkiaSolidBrush : ISolidBrush
	{
		internal SKPaint NativePaint { get; }

		internal SkiaSolidBrush(Color color)
		{
			NativePaint = new SKPaint
			{
				Style = SKPaintStyle.Fill,
				Color = SkiaConvert.ToSKColor(color),
				IsAntialias = true,
			};
		}

		public Color Color
		{
			get => SkiaConvert.ToColor(NativePaint.Color);
			set => NativePaint.Color = SkiaConvert.ToSKColor(value);
		}

		public ISolidBrush Clone() => new SkiaSolidBrush(Color);

		public void Dispose() => NativePaint.Dispose();
	}

	/// <summary>
	/// Shared gradient math for <see cref="SkiaLinearGradientBrush"/>/<see cref="SkiaPathGradientBrush"/>
	/// (Milestone E1, 2026-07-23) — colour-stop construction from GDI+'s <see cref="Blend"/>/
	/// <see cref="ColorBlend"/> shapes, since SkiaSharp's gradient shaders only take a flat
	/// colour/position array, not GDI+'s two alternative blend representations.
	/// </summary>
	internal static class SkiaGradientHelpers
	{
		/// <summary>
		/// Builds the colour/position stop arrays a <c>SKShader.Create*Gradient</c> call needs.
		/// <paramref name="interpolationColors"/> (GDI+'s <c>ColorBlend</c> — an explicit colour/position
		/// list) wins if set; else <paramref name="blend"/> (GDI+'s <c>Blend</c> — a blend-*factor*/position
		/// list, interpreted as the interpolation fraction between <paramref name="startColor"/> and
		/// <paramref name="endColor"/>) is expanded into actual colours; else a plain 2-stop gradient.
		/// </summary>
		internal static SKColor[] BuildColorStops(Color startColor, Color endColor, Blend blend, ColorBlend interpolationColors, out float[] positions)
		{
			if (interpolationColors?.Colors is { Length: > 0 } stopColors)
			{
				positions = interpolationColors.Positions is { Length: > 0 } stopPositions ? (float[])stopPositions.Clone() : EvenPositions(stopColors.Length);
				return Array.ConvertAll(stopColors, SkiaConvert.ToSKColor);
			}
			if (blend?.Factors is { Length: > 0 } factors)
			{
				positions = blend.Positions is { Length: > 0 } blendPositions ? (float[])blendPositions.Clone() : EvenPositions(factors.Length);
				SKColor[] colors = new SKColor[factors.Length];
				for (int i = 0; i < factors.Length; i++)
				{
					colors[i] = SkiaConvert.ToSKColor(LerpColor(startColor, endColor, factors[i]));
				}
				return colors;
			}
			positions = new float[] { 0f, 1f };
			return new[] { SkiaConvert.ToSKColor(startColor), SkiaConvert.ToSKColor(endColor) };
		}

		private static float[] EvenPositions(int count)
		{
			if (count <= 1)
			{
				return new float[count];
			}
			float[] positions = new float[count];
			for (int i = 0; i < count; i++)
			{
				positions[i] = (float)i / (count - 1);
			}
			return positions;
		}

		private static Color LerpColor(Color a, Color b, float t)
		{
			t = Math.Max(0f, Math.Min(1f, t));
			return Color.FromArgb(
				(int)Math.Round(a.A + (b.A - a.A) * t),
				(int)Math.Round(a.R + (b.R - a.R) * t),
				(int)Math.Round(a.G + (b.G - a.G) * t),
				(int)Math.Round(a.B + (b.B - a.B) * t));
		}

		/// <summary>
		/// Approximates GDI+ <c>PathGradientBrush.FocusScales</c> — the size of the fully-<c>CenterColor</c>
		/// region around the focus point — by pushing every stop position outward from 0 by the scales'
		/// average. Exact only for <c>FocusScales == (0,0)</c> (the common case in this codebase); GDI+'s
		/// real per-axis elliptical focus shape has no equivalent in <see cref="SKShader.CreateRadialGradient"/>'s
		/// single-radius model.
		/// </summary>
		internal static void ApplyFocusScale(float[] positions, PointF focusScales)
		{
			float focus = Math.Max(0f, Math.Min(1f, (focusScales.X + focusScales.Y) / 2f));
			if (focus <= 0f)
			{
				return;
			}
			for (int i = 0; i < positions.Length; i++)
			{
				positions[i] = focus + positions[i] * (1f - focus);
			}
		}

		/// <summary>GDI+'s <c>TileFlipX/Y/XY</c> all alternate tiles; a single-axis gradient shader only has one tile direction, so all three collapse to <see cref="SKShaderTileMode.Mirror"/>.</summary>
		internal static SKShaderTileMode ToTileMode(WrapMode mode) => mode switch
		{
			WrapMode.Clamp => SKShaderTileMode.Clamp,
			WrapMode.Tile => SKShaderTileMode.Repeat,
			_ => SKShaderTileMode.Mirror,
		};
	}

	/// <summary>
	/// Real adapter (Milestone E1, 2026-07-23) — wraps a fill-styled <see cref="SKPaint"/> whose
	/// <see cref="SKPaint.Shader"/> is <see cref="SKShader.CreateLinearGradient(SKPoint, SKPoint, SKColor[], float[], SKShaderTileMode, in SKMatrix)"/>.
	/// The native paint is rebuilt lazily on next access after any property setter, mirroring
	/// <see cref="Gdi.GdiLinearGradientBrush"/>'s live-mutation semantics (GDI+'s <c>LinearGradientBrush</c>
	/// properties write straight through to the native brush; here there's no native brush until a fill
	/// actually needs one, so mutation just marks the cached <see cref="SKPaint"/> stale).
	/// </summary>
	internal sealed class SkiaLinearGradientBrush : ILinearGradientBrush
	{
		private readonly PointF point1;
		private readonly PointF point2;
		private Blend blend;
		private ColorBlend interpolationColors;
		private WrapMode wrapMode = WrapMode.Tile;
		private SKMatrix transform = SKMatrix.Identity;
		private SKPaint cachedPaint;
		private bool dirty = true;

		internal SkiaLinearGradientBrush(RectangleF rect, Color color1, Color color2, float angle)
		{
			LinearColors = new[] { color1, color2 };
			(point1, point2) = GradientLineForAngle(rect, angle);
		}

		internal SkiaLinearGradientBrush(PointF point1, PointF point2, Color color1, Color color2)
		{
			LinearColors = new[] { color1, color2 };
			this.point1 = point1;
			this.point2 = point2;
		}

		/// <summary>
		/// Approximates GDI+'s <c>LinearGradientBrush(RectangleF, Color, Color, float angle)</c> gradient
		/// line: projects the rectangle's corners onto the angle's direction vector and spans exactly that
		/// extent, centred on the rectangle. Exact for the axis-aligned angles (0°/90°) this codebase's real
		/// callers use (<c>ChartGraphics.GetGradientBrushResource</c>'s <c>LeftRight</c>/<c>TopBottom</c>/
		/// <c>*Center</c> cases); an approximation (not a byte-for-byte port, same category as this file's
		/// texture-brush colour-key note) for the <c>DiagonalLeft</c>/<c>DiagonalRight</c> cases.
		/// </summary>
		private static (PointF, PointF) GradientLineForAngle(RectangleF rect, float angleDegrees)
		{
			double radians = angleDegrees * Math.PI / 180.0;
			float dirX = (float)Math.Cos(radians);
			float dirY = (float)Math.Sin(radians);
			PointF center = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
			Span<PointF> corners = stackalloc PointF[4]
			{
				new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Top),
				new PointF(rect.Right, rect.Bottom), new PointF(rect.Left, rect.Bottom),
			};
			float minProjection = float.MaxValue;
			float maxProjection = float.MinValue;
			foreach (PointF corner in corners)
			{
				float projection = (corner.X - center.X) * dirX + (corner.Y - center.Y) * dirY;
				minProjection = Math.Min(minProjection, projection);
				maxProjection = Math.Max(maxProjection, projection);
			}
			PointF start = new PointF(center.X + dirX * minProjection, center.Y + dirY * minProjection);
			PointF end = new PointF(center.X + dirX * maxProjection, center.Y + dirY * maxProjection);
			return (start, end);
		}

		public Blend Blend { get => blend; set { blend = value; dirty = true; } }

		public ColorBlend InterpolationColors { get => interpolationColors; set { interpolationColors = value; dirty = true; } }

		public WrapMode WrapMode { get => wrapMode; set { wrapMode = value; dirty = true; } }

		public Color[] LinearColors { get; }

		internal SKPaint NativePaint
		{
			get
			{
				if (dirty)
				{
					cachedPaint?.Dispose();
					cachedPaint = new SKPaint
					{
						Style = SKPaintStyle.Fill,
						IsAntialias = true,
						Shader = BuildShader(),
					};
					dirty = false;
				}
				return cachedPaint;
			}
		}

		private SKShader BuildShader()
		{
			SKColor[] colors = SkiaGradientHelpers.BuildColorStops(LinearColors[0], LinearColors[1], blend, interpolationColors, out float[] positions);
			return SKShader.CreateLinearGradient(SkiaConvert.ToSKPoint(point1), SkiaConvert.ToSKPoint(point2), colors, positions, SkiaGradientHelpers.ToTileMode(wrapMode), transform);
		}

		public void SetRotationTransform(float angle, PointF center)
		{
			transform = SKMatrix.CreateRotationDegrees(angle, center.X, center.Y);
			dirty = true;
		}

		public void RotateTransform(float angle, MatrixOrder order) => Compose(SKMatrix.CreateRotationDegrees(angle), order);

		public void TranslateTransform(float dx, float dy, MatrixOrder order) => Compose(SKMatrix.CreateTranslation(dx, dy), order);

		public void MultiplyTransform(Matrix3x2 matrix, MatrixOrder order) => Compose(SkiaConvert.ToSKMatrix(matrix), order);

		/// <summary>Mirrors GDI+'s <c>Matrix.Multiply(matrix, order)</c>: <see cref="MatrixOrder.Prepend"/> applies <paramref name="delta"/> before the existing transform, <see cref="MatrixOrder.Append"/> after. Not reachable from the Chart engine's own real call paths today (see IBrush.cs's transform-method docs — Gauge-only), so exactness here is unverified against a real caller.</summary>
		private void Compose(SKMatrix delta, MatrixOrder order)
		{
			transform = order == MatrixOrder.Append ? SKMatrix.Concat(transform, delta) : SKMatrix.Concat(delta, transform);
			dirty = true;
		}

		public void Dispose() => cachedPaint?.Dispose();
	}

	/// <summary>
	/// Real adapter (Milestone E1, 2026-07-23) — approximates GDI+'s <c>PathGradientBrush</c> (a fill
	/// that radiates outward from a focus point to per-boundary-point surround colours) via
	/// <see cref="SKShader.CreateRadialGradient(SKPoint, float, SKColor[], float[], SKShaderTileMode, in SKMatrix)"/>
	/// centred on the source path's bounds. Exact for the circular/elliptical shadow-glow paths this
	/// codebase's real callers build (<c>ChartGraphics.DrawPieSoftShadow</c>/<c>DrawMarkerAbs</c>'s soft
	/// marker shadow/<c>DrawPieGradientEffects</c>'s <c>SoftEdge</c> style, all ellipse paths with a single
	/// surround colour); an approximation for non-elliptical paths or multiple surround colours, since
	/// GDI+'s true per-boundary-point interpolation has no single-shader Skia equivalent.
	/// </summary>
	internal sealed class SkiaPathGradientBrush : IPathGradientBrush
	{
		private readonly RectangleF bounds;
		private Color centerColor;
		private Color[] surroundColors = Array.Empty<Color>();
		private PointF? centerPointOverride;
		private PointF focusScales;
		private Blend blend;
		private ColorBlend interpolationColors;
		private SKMatrix transform = SKMatrix.Identity;
		private SKPaint cachedPaint;
		private bool dirty = true;

		internal SkiaPathGradientBrush(IGraphicsPath path)
		{
			bounds = path.GetBounds();
		}

		public Color CenterColor { get => centerColor; set { centerColor = value; dirty = true; } }

		public Color[] SurroundColors { get => surroundColors; set { surroundColors = value ?? Array.Empty<Color>(); dirty = true; } }

		/// <summary>Defaults to the source path's bounds centre, same as GDI+'s <c>PathGradientBrush.CenterPoint</c> default (the path's centroid — approximated here by its bounding-box centre).</summary>
		public PointF CenterPoint
		{
			get => centerPointOverride ?? new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
			set { centerPointOverride = value; dirty = true; }
		}

		public PointF FocusScales { get => focusScales; set { focusScales = value; dirty = true; } }

		public Blend Blend { get => blend; set { blend = value; dirty = true; } }

		public ColorBlend InterpolationColors { get => interpolationColors; set { interpolationColors = value; dirty = true; } }

		internal SKPaint NativePaint
		{
			get
			{
				if (dirty)
				{
					cachedPaint?.Dispose();
					cachedPaint = new SKPaint
					{
						Style = SKPaintStyle.Fill,
						IsAntialias = true,
						Shader = BuildShader(),
					};
					dirty = false;
				}
				return cachedPaint;
			}
		}

		private SKShader BuildShader()
		{
			Color surroundColor = surroundColors.Length > 0 ? surroundColors[0] : Color.Transparent;
			SKColor[] colors = SkiaGradientHelpers.BuildColorStops(centerColor, surroundColor, blend, interpolationColors, out float[] positions);
			SkiaGradientHelpers.ApplyFocusScale(positions, focusScales);
			float radius = Math.Max(0.5f, Math.Max(bounds.Width, bounds.Height) / 2f);
			return SKShader.CreateRadialGradient(SkiaConvert.ToSKPoint(CenterPoint), radius, colors, positions, SKShaderTileMode.Clamp, transform);
		}

		public void SetRotationTransform(float angle, PointF center)
		{
			transform = SKMatrix.CreateRotationDegrees(angle, center.X, center.Y);
			dirty = true;
		}

		public void RotateTransform(float angle, MatrixOrder order) => Compose(SKMatrix.CreateRotationDegrees(angle), order);

		public void TranslateTransform(float dx, float dy, MatrixOrder order) => Compose(SKMatrix.CreateTranslation(dx, dy), order);

		public void MultiplyTransform(Matrix3x2 matrix, MatrixOrder order) => Compose(SkiaConvert.ToSKMatrix(matrix), order);

		/// <summary>See <see cref="SkiaLinearGradientBrush"/>'s identical helper — same not-reachable-from-Chart caveat.</summary>
		private void Compose(SKMatrix delta, MatrixOrder order)
		{
			transform = order == MatrixOrder.Append ? SKMatrix.Concat(transform, delta) : SKMatrix.Concat(delta, transform);
			dirty = true;
		}

		public void Dispose() => cachedPaint?.Dispose();
	}

	/// <summary>
	/// Real adapter (E1) — wraps a fill-styled <see cref="SKPaint"/> whose <see cref="SKPaint.Shader"/>
	/// is a bitmap shader (<see cref="SKShader.CreateBitmap(SKBitmap, SKShaderTileMode, SKShaderTileMode, SKMatrix)"/>),
	/// built by <see cref="SkiaResourceFactory.CreateTextureBrush(IChartImage, WrapMode)"/>/
	/// <see cref="SkiaResourceFactory.CreateTextureBrush(IChartImage, RectangleF, IImageDrawOptions)"/>.
	/// Owns an optional colour-keyed bitmap copy (see <c>CreateTextureBrush(image, rect, options)</c>),
	/// disposed alongside the paint.
	/// </summary>
	internal sealed class SkiaTextureBrush : ITextureBrush
	{
		internal SKPaint NativePaint { get; }

		private readonly SKBitmap ownedBitmap;

		public WrapMode WrapMode { get; set; }

		internal SkiaTextureBrush(SKPaint paint, WrapMode wrapMode, SKBitmap ownedBitmap = null)
		{
			NativePaint = paint;
			WrapMode = wrapMode;
			this.ownedBitmap = ownedBitmap;
		}

		public void Dispose()
		{
			NativePaint.Dispose();
			ownedBitmap?.Dispose();
		}
	}

	/// <summary>
	/// Real (Milestone E1, 2026-07-23) — a fill-styled <see cref="SKPaint"/> whose <see cref="SKPaint.Shader"/>
	/// tiles a small bitmap generated by <see cref="SkiaHatchPatterns.BuildTile"/> (see that type for the
	/// approximation rationale — GDI+'s hatch bitmaps aren't portable, so every style is a documented
	/// pixel-predicate approximation rather than an exact port).
	/// </summary>
	internal sealed class SkiaHatchBrush : IHatchBrush
	{
		internal SKPaint NativePaint { get; }

		public HatchStyle HatchStyle { get; }
		public Color ForegroundColor { get; }
		public Color BackgroundColor { get; }

		internal SkiaHatchBrush(HatchStyle style, Color foreColor, Color backColor)
		{
			HatchStyle = style;
			ForegroundColor = foreColor;
			BackgroundColor = backColor;
			using SKBitmap tile = SkiaHatchPatterns.BuildTile(style, SkiaConvert.ToSKColor(foreColor), SkiaConvert.ToSKColor(backColor));
			NativePaint = new SKPaint
			{
				Style = SKPaintStyle.Fill,
				Shader = SKShader.CreateBitmap(tile, SKShaderTileMode.Repeat, SKShaderTileMode.Repeat),
			};
		}

		public void Dispose() => NativePaint.Dispose();
	}
}
