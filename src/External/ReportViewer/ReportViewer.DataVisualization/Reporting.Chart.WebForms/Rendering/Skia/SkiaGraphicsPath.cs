using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using SkiaSharp;
using Reporting.Rendering;

namespace Reporting.Chart.WebForms.Rendering.Skia
{
	/// <summary>
	/// Spike adapter — wraps an <see cref="SKPath"/> behind <see cref="IGraphicsPath"/>.
	/// Covers the operations the sample scene exercises (lines/rectangles/ellipses/polygons,
	/// figure control, transform, bounds/hit-test). The remaining Appendix A.1 members
	/// (<c>AddCurve</c>/<c>AddClosedCurve</c>/<c>AddPath</c>/<c>AddString</c>/<c>Widen</c>/
	/// <c>Reverse</c>/<c>SetMarkers</c>/<c>Flatten</c>) have direct-enough Skia equivalents
	/// (<c>SKPath.Op</c>, <c>SKPaint.GetFillPath</c>, ...) but translating them precisely is
	/// real Milestone E1 work — stubbed here since the spike scene never calls them.
	/// </summary>
	internal sealed class SkiaGraphicsPath : IGraphicsPath
	{
		internal SKPath NativePath { get; } = new SKPath();

		internal SkiaGraphicsPath()
		{
		}

		/// <summary>
		/// Real (Milestone E2, 2026-07-23) — genuinely reachable via <see cref="SkiaResourceFactory.CreatePath(PointF[], byte[])"/>
		/// (<c>CalloutAnnotation.SplitAtMarkers</c>/<c>DrawCloudCallout</c> rebuild sub-paths from a parent
		/// path's <see cref="PathPoints"/>/<see cref="PathTypes"/>). The exact inverse of <see cref="PathTypes"/>'s
		/// encoding: a Start point begins a new contour (<c>MoveTo</c>), a Line point appends a line, three
		/// consecutive Bezier points append one cubic, and the <c>CloseSubpath</c> flag (read off whichever
		/// point it was OR-ed onto — the last point added to that contour) closes it.
		/// </summary>
		internal SkiaGraphicsPath(PointF[] points, byte[] types)
		{
			int i = 0;
			while (i < points.Length)
			{
				if ((types[i] & (byte)PathPointType.PathTypeMask) != (byte)PathPointType.Start)
				{
					throw new ArgumentException($"Expected PathPointType.Start at index {i}.");
				}
				NativePath.MoveTo(SkiaConvert.ToSKPoint(points[i]));
				bool closeSubpath = (types[i] & (byte)PathPointType.CloseSubpath) != 0;
				i++;
				while (i < points.Length && (types[i] & (byte)PathPointType.PathTypeMask) != (byte)PathPointType.Start)
				{
					byte kind = (byte)(types[i] & (byte)PathPointType.PathTypeMask);
					if (kind == (byte)PathPointType.Bezier)
					{
						NativePath.CubicTo(SkiaConvert.ToSKPoint(points[i]), SkiaConvert.ToSKPoint(points[i + 1]), SkiaConvert.ToSKPoint(points[i + 2]));
						closeSubpath = (types[i + 2] & (byte)PathPointType.CloseSubpath) != 0;
						i += 3;
					}
					else
					{
						NativePath.LineTo(SkiaConvert.ToSKPoint(points[i]));
						closeSubpath = (types[i] & (byte)PathPointType.CloseSubpath) != 0;
						i++;
					}
				}
				if (closeSubpath)
				{
					NativePath.Close();
				}
			}
		}

		public FillMode FillMode
		{
			get => NativePath.FillType == SKPathFillType.EvenOdd ? FillMode.Alternate : FillMode.Winding;
			set => NativePath.FillType = value == FillMode.Alternate ? SKPathFillType.EvenOdd : SKPathFillType.Winding;
		}

		public PointF[] PathPoints
		{
			get
			{
				var points = NativePath.Points;
				var result = new PointF[points.Length];
				for (var i = 0; i < points.Length; i++)
				{
					result[i] = new PointF(points[i].X, points[i].Y);
				}
				return result;
			}
		}

		/// <summary>
		/// Real (Milestone E2, 2026-07-23) — genuinely reachable via <c>HotRegionsList.AddHotRegion(IGraphicsPath,...)</c>,
		/// which bridges back to a concrete <see cref="GraphicsPath"/> for hit-testing storage (deliberately
		/// concrete-only regardless of backend, same as the Gauge engine's <c>HotRegionList</c>) by feeding
		/// <see cref="PathPoints"/>/<see cref="PathTypes"/> straight into <c>new GraphicsPath(points, types)</c> —
		/// which throws if the two arrays don't line up 1:1. Walks <see cref="SKPath"/>'s verb iterator and maps
		/// each verb to GDI+'s <see cref="PathPointType"/> byte encoding (Move → Start, Line → Line, Cubic → 3×
		/// Bezier, Close → OR the CloseSubpath flag onto the previous point, no new point). Quad/Conic segments
		/// (produced internally by <c>AddEllipse</c>/<c>AddArc</c>) have no direct GDI+ equivalent and aren't hit
		/// by any current caller of this method — left as a documented gap (throws) rather than silently wrong,
		/// same precedent as this file's other Appendix A.1 stubs.
		/// </summary>
		public byte[] PathTypes
		{
			get
			{
				var types = new System.Collections.Generic.List<byte>(NativePath.PointCount);
				var verbPoints = new SKPoint[4];
				using var iterator = NativePath.CreateRawIterator();
				SKPathVerb verb;
				while ((verb = iterator.Next(verbPoints)) != SKPathVerb.Done)
				{
					switch (verb)
					{
						case SKPathVerb.Move:
							types.Add((byte)PathPointType.Start);
							break;
						case SKPathVerb.Line:
							types.Add((byte)PathPointType.Line);
							break;
						case SKPathVerb.Cubic:
							types.Add((byte)PathPointType.Bezier);
							types.Add((byte)PathPointType.Bezier);
							types.Add((byte)PathPointType.Bezier);
							break;
						case SKPathVerb.Close:
							if (types.Count > 0)
							{
								types[types.Count - 1] = (byte)(types[types.Count - 1] | (byte)PathPointType.CloseSubpath);
							}
							break;
						case SKPathVerb.Quad:
						case SKPathVerb.Conic:
							// Documented approximation (Milestone E2, 2026-07-23): GDI+'s PathPointType has no
							// rational/quadratic-Bezier member, so a Quad/Conic's 2 new points (control, end —
							// verbPoints[0] is the already-counted current point, same as Cubic above) are
							// approximated as 2 line vertices. Only affects HotRegionsList's concrete
							// GraphicsPath hit-test hull (e.g. TextAnnotation/EllipseAnnotation's hot region),
							// never actual rendering, so the minor hull imprecision on curved edges is inert.
							types.Add((byte)PathPointType.Line);
							types.Add((byte)PathPointType.Line);
							break;
						default:
							throw new NotImplementedException($"SkiaGraphicsPath.PathTypes: {verb} segments have no GDI+ PathPointType equivalent implemented yet.");
					}
				}
				return types.ToArray();
			}
		}

		public int PointCount => NativePath.PointCount;

		/// <summary>
		/// Real (Milestone E2, 2026-07-23) fix — was unconditionally <c>MoveTo(pt1); LineTo(pt2)</c>, which
		/// starts a brand-new disconnected contour on every call. GDI+'s <c>GraphicsPath.AddLine</c> instead
		/// continues the current figure when one is already open (the overwhelmingly common real usage:
		/// callers chain <c>AddLine(prev, next)</c> calls where each <c>pt1</c> equals the previous call's
		/// <c>pt2</c>, e.g. <c>RangeChart.FillLastSeriesGradient</c>'s whole-series boundary walk) — without
		/// this, such chains became N disconnected 2-point segments instead of one continuous polyline,
		/// breaking any fill that depends on the boundary being a single closed shape. <see cref="openFigure"/>
		/// tracks whether the current contour is still an open line/curve chain eligible to continue.
		/// </summary>
		public void AddLine(PointF pt1, PointF pt2)
		{
			if (openFigure)
			{
				NativePath.LineTo(SkiaConvert.ToSKPoint(pt1));
			}
			else
			{
				NativePath.MoveTo(SkiaConvert.ToSKPoint(pt1));
			}
			NativePath.LineTo(SkiaConvert.ToSKPoint(pt2));
			openFigure = true;
		}

		/// <summary>See <see cref="AddLine(PointF, PointF)"/>'s doc comment — tracks whether the current contour is an open line/curve chain that a subsequent <c>AddLine</c> call should continue rather than restart.</summary>
		private bool openFigure;

		public void AddLine(float x1, float y1, float x2, float y2) => AddLine(new PointF(x1, y1), new PointF(x2, y2));

		public void AddLines(PointF[] points)
		{
			if (points.Length == 0)
			{
				return;
			}
			NativePath.MoveTo(SkiaConvert.ToSKPoint(points[0]));
			for (var i = 1; i < points.Length; i++)
			{
				NativePath.LineTo(SkiaConvert.ToSKPoint(points[i]));
			}
		}

		/// <summary>
		/// Real correctness fix (Milestone B1b, 2026-07-26) — same class of bug as <see cref="AddLine(PointF, PointF)"/>'s
		/// fix above. <c>SKPath.AddArc</c> always starts a brand-new contour (an implicit <c>MoveTo</c>), but GDI+'s
		/// <c>GraphicsPath.AddArc</c> continues the current figure — connecting the last point with a straight line to
		/// the arc's start — when one is already open (e.g. <c>ChartGraphics3D.FillDoughnutSlice</c>'s
		/// <c>AddLine(...); AddArc(...); AddLine(...); AddArc(...)</c> chain, which needs one continuous closed boundary
		/// for the doughnut-hole fill). <c>SKPath.ArcTo(rect, startAngle, sweepAngle, forceMoveTo: false)</c> has exactly
		/// GDI+'s semantics (falls back to an implicit initial <c>MoveTo</c> only when the path is still empty) —
		/// <see cref="AddPie(float, float, float, float, float, float)"/> already used this correctly below, just not
		/// this method. Caught the same way the <c>AddLine</c> bug was: no exception, only visible on eyeballing the
		/// rendered PNG (self-intersecting "bowtie" fill artifacts inside a 3D doughnut's inner hole boundary).
		/// </summary>
		public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			NativePath.ArcTo(new SKRect(x, y, x + width, y + height), startAngle, sweepAngle, forceMoveTo: false);
			openFigure = true;
		}

		public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4)
		{
			NativePath.MoveTo(SkiaConvert.ToSKPoint(pt1));
			NativePath.CubicTo(SkiaConvert.ToSKPoint(pt2), SkiaConvert.ToSKPoint(pt3), SkiaConvert.ToSKPoint(pt4));
		}

		public void AddBeziers(PointF[] points) => throw new NotImplementedException("Spike scope: not exercised by the sample scene.");

		public void AddCurve(PointF[] points, float tension) => throw new NotImplementedException("Spike scope: not exercised by the sample scene.");

		public void AddCurve(PointF[] points, int offset, int numberOfSegments, float tension) => throw new NotImplementedException("Spike scope: not exercised by the sample scene.");

		public void AddClosedCurve(PointF[] points) => throw new NotImplementedException("Spike scope: not exercised by the sample scene.");

		public void AddClosedCurve(PointF[] points, float tension) => throw new NotImplementedException("Spike scope: not exercised by the sample scene.");

		public void AddEllipse(float x, float y, float width, float height) => NativePath.AddOval(new SKRect(x, y, x + width, y + height));

		public void AddEllipse(RectangleF rect) => NativePath.AddOval(SkiaConvert.ToSKRect(rect));

		public void AddRectangle(RectangleF rect) => NativePath.AddRect(SkiaConvert.ToSKRect(rect));

		public void AddPolygon(PointF[] points)
		{
			var skPoints = new SKPoint[points.Length];
			for (var i = 0; i < points.Length; i++)
			{
				skPoints[i] = SkiaConvert.ToSKPoint(points[i]);
			}
			NativePath.AddPoly(skPoints, close: true);
		}

		public void AddPie(float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			var rect = new SKRect(x, y, x + width, y + height);
			NativePath.MoveTo(rect.MidX, rect.MidY);
			NativePath.ArcTo(rect, startAngle, sweepAngle, forceMoveTo: false);
			NativePath.Close();
		}

		public void AddPath(IGraphicsPath addingPath, bool connect) => NativePath.AddPath(((SkiaGraphicsPath)addingPath).NativePath, connect ? SKPathAddMode.Extend : SKPathAddMode.Append);

		/// <summary>Real (Milestone E2, 2026-07-23) — genuinely reachable (<c>TextAnnotation</c>/<c>Title</c>'s <c>TextStyle.Frame</c> path). See <see cref="AddString(string, IChartFont, RectangleF, ITextFormat)"/>.</summary>
		public void AddString(string text, IChartFont font, PointF origin, ITextFormat format)
		{
			var skFont = ((SkiaChartFont)font).NativeFont;
			skFont.GetFontMetrics(out var metrics);
			float baselineY = origin.Y - metrics.Ascent;
			AppendGlyphOutlines(text, skFont, origin.X, baselineY);
		}

		/// <summary>
		/// Real (Milestone E2, 2026-07-23) — genuinely reachable (<c>TextAnnotation</c>/<c>Title</c>'s
		/// <c>TextStyle.Frame</c> path calls <c>ChartGraphics.GetTextPath</c>, which round-trips through
		/// this). GDI+'s <c>GraphicsPath.AddString</c> has no single-call Skia equivalent, so this walks
		/// each glyph's own outline (<see cref="SKFont.GetGlyphPath(ushort)"/>) and appends it translated
		/// to its advance position — the same technique <c>SKPaint.GetTextPath</c> used before the
		/// paint/font API split. Alignment mirrors <see cref="SkiaChartGraphics.DrawAlignedString"/>'s.
		/// Splits on <c>'\n'</c> and stacks lines vertically (needed by <c>ChartGraphics.GetStackedText</c>'s
		/// one-character-per-line encoding for <c>TextStyle.Frame</c>) — without this, every "line" would
		/// land on the same baseline and overlap, since GDI+'s single-call <c>AddString</c> line-breaks
		/// internally but this backend's per-glyph walk otherwise never would.
		/// </summary>
		public void AddString(string text, IChartFont font, RectangleF layoutRect, ITextFormat format)
		{
			var skFont = ((SkiaChartFont)font).NativeFont;
			skFont.GetFontMetrics(out var metrics);
			float lineHeight = metrics.Descent - metrics.Ascent + metrics.Leading;

			string[] lines = text.Split('\n');
			float blockHeight = lineHeight * lines.Length;

			var alignment = format?.Alignment ?? StringAlignment.Near;
			var lineAlignment = format?.LineAlignment ?? StringAlignment.Near;

			float blockTop = lineAlignment switch
			{
				StringAlignment.Center => layoutRect.Y + (layoutRect.Height - blockHeight) / 2f,
				StringAlignment.Far => layoutRect.Y + layoutRect.Height - blockHeight,
				_ => layoutRect.Y,
			};

			for (int line = 0; line < lines.Length; line++)
			{
				float lineWidth = skFont.MeasureText(lines[line]);
				float x = alignment switch
				{
					StringAlignment.Center => layoutRect.X + (layoutRect.Width - lineWidth) / 2f,
					StringAlignment.Far => layoutRect.X + layoutRect.Width - lineWidth,
					_ => layoutRect.X,
				};
				float top = blockTop + lineHeight * line;
				float baselineY = top - metrics.Ascent;
				AppendGlyphOutlines(lines[line], skFont, x, baselineY);
			}
		}

		private void AppendGlyphOutlines(string text, SKFont skFont, float x, float baselineY)
		{
			ushort[] glyphs = skFont.GetGlyphs(text);
			float[] widths = skFont.GetGlyphWidths(glyphs);
			float cursor = x;
			for (int i = 0; i < glyphs.Length; i++)
			{
				using SKPath glyphPath = skFont.GetGlyphPath(glyphs[i]);
				if (glyphPath != null && !glyphPath.IsEmpty)
				{
					glyphPath.Transform(SKMatrix.CreateTranslation(cursor, baselineY));
					NativePath.AddPath(glyphPath, SKPathAddMode.Append);
				}
				cursor += widths[i];
			}
		}

		public void StartFigure()
		{
			// SKPath has no explicit figure marker to "start" ahead of the next Add* call
			// (each Add* implicitly starts a new contour with its own MoveTo); ends the current
			// AddLine chain (see openFigure) so the next AddLine call starts fresh rather than
			// wrongly continuing whatever figure came before.
			openFigure = false;
		}

		public void CloseFigure()
		{
			NativePath.Close();
			openFigure = false;
		}

		public void CloseAllFigures()
		{
			NativePath.Close();
			openFigure = false;
		}

		/// <summary>Real (Milestone E2, 2026-07-23) — genuinely reachable (<c>CalloutAnnotation.DrawRoundedRectCallout</c>, to walk a rounded rect's/ellipse's vertices as a plain polygon). See <see cref="Flatten(float)"/>.</summary>
		public void Flatten() => Flatten(0.25f);

		/// <summary>
		/// Real (Milestone E2, 2026-07-23) — approximates GDI+'s <c>GraphicsPath.Flatten(float)</c> (replace
		/// every curve segment with a line-segment approximation within <paramref name="flatness"/> of the
		/// true curve) via <see cref="SKPathMeasure"/>, sampling each contour every <paramref name="flatness"/>
		/// device units (matching GDI+'s flatness-as-max-deviation contract closely enough for the callers
		/// here, which only re-walk <see cref="PathPoints"/> afterward — e.g. to relocate the nearest vertex
		/// toward a callout's anchor point). SkiaSharp has no direct "flatten in place" primitive.
		/// </summary>
		public void Flatten(float flatness)
		{
			float step = Math.Max(flatness, 0.01f);
			using var measure = new SKPathMeasure(NativePath, forceClosed: false);
			var flattened = new SKPath();
			do
			{
				float length = measure.Length;
				int steps = Math.Max(1, (int)Math.Ceiling(length / step));
				for (int s = 0; s <= steps; s++)
				{
					float distance = length * s / steps;
					if (measure.GetPosition(distance, out SKPoint point))
					{
						if (s == 0)
						{
							flattened.MoveTo(point);
						}
						else
						{
							flattened.LineTo(point);
						}
					}
				}
				if (measure.IsClosed)
				{
					flattened.Close();
				}
			}
			while (measure.NextContour());
			NativePath.Reset();
			NativePath.AddPath(flattened, SKPathAddMode.Append);
			flattened.Dispose();
		}

		/// <summary>
		/// Real implementation (not spike-scoped) — GDI+'s <c>GraphicsPath.Widen(Pen)</c> converts
		/// a stroked path into the filled outline geometry of that stroke. <see cref="SKPaint.GetFillPath"/>
		/// is Skia's direct equivalent (used by Skia internally to rasterize strokes), so this needs
		/// no hand-rolled geometry algorithm.
		/// </summary>
		public void Widen(IPen pen)
		{
			using SKPaint strokePaint = new SKPaint
			{
				Style = SKPaintStyle.Stroke,
				StrokeWidth = pen.Width,
				StrokeCap = SkiaConvert.ToSKStrokeCap(pen.StartCap),
				StrokeJoin = SkiaConvert.ToSKStrokeJoin(pen.LineJoin),
			};
			using SKPath widened = new SKPath();
			strokePaint.GetFillPath(NativePath, widened);
			NativePath.Reset();
			NativePath.AddPath(widened, SKPathAddMode.Append);
		}

		/// <summary>
		/// Real (Milestone E2, 2026-07-23) — genuinely reachable (<c>RangeChart.FillLastSeriesGradient</c>
		/// reverses the "bottom" boundary path before stitching it to the "top" boundary path into one
		/// closed gradient-fill polygon). Reverses both the order of subpaths and the point order within
		/// each (matching GDI+'s <c>GraphicsPath.Reverse</c>), reusing <see cref="PathPoints"/>/<see cref="PathTypes"/>'
		/// same run-length grouping — for a Bezier run, reversing the whole run's point order and re-emitting
		/// the same 3-point grouping via <c>CubicTo</c> naturally swaps the control-point roles too, which is
		/// exactly the geometrically-correct reversed curve.
		/// </summary>
		public void Reverse()
		{
			PointF[] points = PathPoints;
			byte[] types = PathTypes;
			var subpaths = new System.Collections.Generic.List<(PointF[] pts, System.Collections.Generic.List<(byte kind, int len)> segs, bool closed)>();
			int i = 0;
			while (i < points.Length)
			{
				int start = i;
				bool closed = (types[i] & (byte)PathPointType.CloseSubpath) != 0;
				var segs = new System.Collections.Generic.List<(byte, int)>();
				i++;
				while (i < points.Length && (types[i] & (byte)PathPointType.PathTypeMask) != (byte)PathPointType.Start)
				{
					byte kind = (byte)(types[i] & (byte)PathPointType.PathTypeMask);
					if ((types[i] & (byte)PathPointType.CloseSubpath) != 0)
					{
						closed = true;
					}
					if (kind == (byte)PathPointType.Bezier)
					{
						segs.Add((kind, 3));
						i += 3;
					}
					else
					{
						segs.Add((kind, 1));
						i++;
					}
				}
				var pts = new PointF[i - start];
				Array.Copy(points, start, pts, 0, i - start);
				subpaths.Add((pts, segs, closed));
			}
			subpaths.Reverse();
			NativePath.Reset();
			foreach (var (pts, segs, closed) in subpaths)
			{
				var reversedPts = (PointF[])pts.Clone();
				Array.Reverse(reversedPts);
				segs.Reverse();
				NativePath.MoveTo(SkiaConvert.ToSKPoint(reversedPts[0]));
				int idx = 1;
				foreach (var (kind, len) in segs)
				{
					if (kind == (byte)PathPointType.Bezier)
					{
						NativePath.CubicTo(SkiaConvert.ToSKPoint(reversedPts[idx]), SkiaConvert.ToSKPoint(reversedPts[idx + 1]), SkiaConvert.ToSKPoint(reversedPts[idx + 2]));
						idx += 3;
					}
					else
					{
						NativePath.LineTo(SkiaConvert.ToSKPoint(reversedPts[idx]));
						idx += len;
					}
				}
				if (closed)
				{
					NativePath.Close();
				}
			}
		}

		public void SetMarkers()
		{
			// GDI+-specific path-marker concept with no Skia equivalent; no-op (not read back by the spike scene).
		}

		public void Transform(Matrix3x2 matrix) =>
			NativePath.Transform(new SKMatrix(matrix.M11, matrix.M21, matrix.M31, matrix.M12, matrix.M22, matrix.M32, 0, 0, 1));

		public RectangleF GetBounds()
		{
			var b = NativePath.Bounds;
			return new RectangleF(b.Left, b.Top, b.Width, b.Height);
		}

		public RectangleF GetBounds(Matrix3x2 matrix) => throw new NotImplementedException("Spike scope: not exercised by the sample scene.");

		public bool IsVisible(PointF point) => NativePath.Contains(point.X, point.Y);

		public void Reset()
		{
			NativePath.Reset();
			openFigure = false;
		}

		public void Dispose() => NativePath.Dispose();
	}
}
