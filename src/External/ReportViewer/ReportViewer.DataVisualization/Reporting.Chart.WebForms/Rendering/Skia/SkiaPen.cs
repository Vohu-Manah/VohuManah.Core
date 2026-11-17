using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using SkiaSharp;
using Reporting.Rendering;

namespace Reporting.Chart.WebForms.Rendering.Skia
{
	/// <summary>
	/// Adapter behind <see cref="IPen"/>, wrapping an <see cref="SKPaint"/> configured for
	/// stroking. Unlike <c>GdiPen</c>, this never touches <see cref="System.Drawing"/>, so it
	/// can be constructed on Linux where GDI+ initialization itself throws (see spike report).
	/// Dash/cap/join are real (E1) via <see cref="SKPathEffect.CreateDash"/>/<see cref="SkiaConvert"/>;
	/// <see cref="Alignment"/>/<see cref="DashPattern"/> stay plain properties — Skia strokes are
	/// always center-aligned (no GDI+ <c>PenAlignment.Inset</c> equivalent) and <c>DashPattern</c>
	/// is a caller-facing custom-dash escape hatch not yet exercised by any real caller.
	/// </summary>
	internal sealed class SkiaPen : IPen
	{
		internal SKPaint NativePaint { get; }

		private DashStyle dashStyle;
		private LineCap startCap;
		private LineCap endCap;
		private LineJoin lineJoin = LineJoin.Miter;

		internal SkiaPen(Color color, float width)
		{
			NativePaint = new SKPaint
			{
				Style = SKPaintStyle.Stroke,
				Color = SkiaConvert.ToSKColor(color),
				StrokeWidth = width,
				StrokeJoin = SkiaConvert.ToSKStrokeJoin(lineJoin),
				IsAntialias = true,
			};
		}

		/// <summary>Strokes with an arbitrary brush's fill (solid/texture) — GDI+'s <c>Pen(Brush, float)</c> constructor.</summary>
		internal SkiaPen(SKPaint brushSource, float width)
		{
			NativePaint = new SKPaint
			{
				Style = SKPaintStyle.Stroke,
				Color = brushSource.Color,
				Shader = brushSource.Shader,
				StrokeWidth = width,
				StrokeJoin = SkiaConvert.ToSKStrokeJoin(lineJoin),
				IsAntialias = true,
			};
		}

		public Color Color
		{
			get => SkiaConvert.ToColor(NativePaint.Color);
			set => NativePaint.Color = SkiaConvert.ToSKColor(value);
		}

		public float Width
		{
			get => NativePaint.StrokeWidth;
			set
			{
				NativePaint.StrokeWidth = value;
				ApplyDashStyle();
			}
		}

		public DashStyle DashStyle
		{
			get => dashStyle;
			set
			{
				dashStyle = value;
				ApplyDashStyle();
			}
		}

		public LineCap StartCap
		{
			get => startCap;
			set
			{
				startCap = value;
				// Skia has one stroke cap per paint (no distinct start/end); StartCap is the source of truth,
				// matching how GDI+ pens are used in this codebase (both caps almost always set identically).
				NativePaint.StrokeCap = SkiaConvert.ToSKStrokeCap(value);
			}
		}

		public LineCap EndCap
		{
			get => endCap;
			set => endCap = value;
		}

		public LineJoin LineJoin
		{
			get => lineJoin;
			set
			{
				lineJoin = value;
				NativePaint.StrokeJoin = SkiaConvert.ToSKStrokeJoin(value);
			}
		}

		// No Skia equivalent to GDI+'s PenAlignment.Inset (Skia strokes are always centered on the path);
		// kept as a plain property so callers that only set/read it don't need special-casing per backend.
		public PenAlignment Alignment { get; set; }

		public float MiterLimit
		{
			get => NativePaint.StrokeMiter;
			set => NativePaint.StrokeMiter = value;
		}

		public float[] DashPattern { get; set; }

		private CustomLineCap customStartCap;
		private CustomLineCap customEndCap;

		/// <summary>
		/// Real (Milestone E2, 2026-07-23), documented approximation — genuinely reachable
		/// (<c>CalloutAnnotation</c>'s line-callout arrow caps, via <c>AdjustableArrowCap</c>). Skia has
		/// no custom-line-cap primitive (<see cref="SKPaint"/> only has <see cref="SKStrokeCap"/>'s fixed
		/// Butt/Round/Square set), so the value is stored but not rendered — strokes on this backend keep
		/// whatever <see cref="StartCap"/>/<see cref="EndCap"/> already produced, same "approximate but
		/// honest" precedent as <c>SkiaHatchBrush</c>'s pattern tiles.
		/// </summary>
		public CustomLineCap CustomStartCap { get => customStartCap; set => customStartCap = value; }

		/// <summary>See <see cref="CustomStartCap"/> — same documented no-op approximation.</summary>
		public CustomLineCap CustomEndCap { get => customEndCap; set => customEndCap = value; }

		private void ApplyDashStyle()
		{
			NativePaint.PathEffect?.Dispose();
			float unit = NativePaint.StrokeWidth <= 0f ? 1f : NativePaint.StrokeWidth;
			NativePaint.PathEffect = dashStyle switch
			{
				DashStyle.Dash => SKPathEffect.CreateDash(new[] { 3f * unit, 1f * unit }, 0f),
				DashStyle.Dot => SKPathEffect.CreateDash(new[] { 1f * unit, 1f * unit }, 0f),
				DashStyle.DashDot => SKPathEffect.CreateDash(new[] { 3f * unit, 1f * unit, 1f * unit, 1f * unit }, 0f),
				DashStyle.DashDotDot => SKPathEffect.CreateDash(new[] { 3f * unit, 1f * unit, 1f * unit, 1f * unit, 1f * unit, 1f * unit }, 0f),
				_ => null,
			};
		}

		public IPen Clone()
		{
			SkiaPen skiaPen = new SkiaPen(Color, Width);
			skiaPen.DashStyle = DashStyle;
			skiaPen.StartCap = StartCap;
			skiaPen.EndCap = EndCap;
			skiaPen.LineJoin = LineJoin;
			skiaPen.Alignment = Alignment;
			return skiaPen;
		}

		public void Dispose()
		{
			NativePaint.PathEffect?.Dispose();
			NativePaint.Dispose();
		}
	}
}
