using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using Reporting.Gauge.WebForms.Rendering;
using Reporting.Rendering;

namespace Reporting.Gauge.WebForms
{
	/// <summary>
	/// Milestone B2 open item #7 (tasks/gauge-gdi-type-abstraction.md): pure geometry-building `static`
	/// utility class, converted to <see cref="IGraphicsPath"/> in one atomic pass since its only real
	/// caller, <c>NumericIndicator.DrawSymbol</c>, was the sole consumer of every public entry point
	/// (confirmed by solution-wide grep) — retype in place, no dual overload needed. Since this class has
	/// no instance and hence no <c>ResourceFactory</c> field of its own, every method threads an
	/// <see cref="IGaugeDrawingResourceFactory"/> parameter down from the caller's live <c>GaugeGraphics</c>
	/// instead. Rotation/shear matrices stay built via the native <see cref="Matrix"/> (RotateAt/Shear are
	/// exact, well-understood GDI+ operations) and are only bridged to <see cref="Matrix3x2"/> at the
	/// <see cref="IGraphicsPath.Transform(Matrix3x2)"/> call site itself, via <see cref="ToMatrix3x2"/> —
	/// the same literal 1:1-port convention used by <c>Knob.cs</c>/<c>LinearPointer.cs</c>/<c>ScaleBase.cs</c>.
	/// </summary>
	internal static class DigitalSegment
	{
		internal const float widthRatio = 0.618034f;

		internal const float shearFactor = 0.0618034f;

		internal const float sgmntWidth7 = 0.142857149f;

		private static Matrix3x2 ToMatrix3x2(Matrix nativeMatrix)
		{
			float[] elements = nativeMatrix.Elements;
			return new Matrix3x2(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]);
		}

		private static PointF[] GetSegment7(PointF p, SizeF s)
		{
			return new PointF[6]
			{
				new PointF(p.X - s.Width / 2f, p.Y),
				new PointF(p.X - s.Width / 2f + s.Height / 2f, p.Y - s.Height / 2f),
				new PointF(p.X + s.Width / 2f - s.Height / 2f, p.Y - s.Height / 2f),
				new PointF(p.X + s.Width / 2f, p.Y),
				new PointF(p.X + s.Width / 2f - s.Height / 2f, p.Y + s.Height / 2f),
				new PointF(p.X - s.Width / 2f + s.Height / 2f, p.Y + s.Height / 2f)
			};
		}

		private static PointF[] GetSegmentHKLN(IGaugeDrawingResourceFactory resourceFactory, PointF p, SizeF s, float smallWidth, bool left)
		{
			using (IGraphicsPath graphicsPath = resourceFactory.CreatePath())
			{
				RectangleF rect = new RectangleF(0f, 0f, s.Width / 2f - smallWidth - smallWidth / 6f, smallWidth / 0.618034f);
				rect.X = (0f - rect.Width) / 2f;
				rect.Y = (0f - rect.Height) / 2f;
				graphicsPath.AddRectangle(rect);
				using (Matrix matrix = new Matrix())
				{
					matrix.Shear(0f, left ? 1.3f : (-1.3f));
					graphicsPath.Transform(ToMatrix3x2(matrix));
					matrix.Reset();
					matrix.Translate(p.X, p.Y);
					graphicsPath.Transform(ToMatrix3x2(matrix));
				}
				return (PointF[])graphicsPath.PathPoints.Clone();
			}
		}

		private static IGraphicsPath GetSegment7(IGaugeDrawingResourceFactory resourceFactory, LEDSegment7 segment, PointF p, float size)
		{
			IGraphicsPath graphicsPath = resourceFactory.CreatePath();
			SizeF sizeF = new SizeF(size * 0.618034f, size);
			float num = sizeF.Width * 0.142857149f;
			SizeF s = new SizeF(sizeF.Width - num, num);
			SizeF s2 = new SizeF(sizeF.Height / 2f - num / 2f, num);
			s.Width -= num / 3f;
			s2.Width -= num / 3f;
			switch (segment)
			{
			case LEDSegment7.Empty:
			case LEDSegment7.SA:
			case LEDSegment7.SB:
			case LEDSegment7.SA | LEDSegment7.SB:
			case LEDSegment7.SC:
			case LEDSegment7.SA | LEDSegment7.SC:
			case LEDSegment7.N1:
			case LEDSegment7.N7:
			case LEDSegment7.SD:
			case LEDSegment7.SA | LEDSegment7.SD:
			case LEDSegment7.SB | LEDSegment7.SD:
			case LEDSegment7.SA | LEDSegment7.SB | LEDSegment7.SD:
			case LEDSegment7.SC | LEDSegment7.SD:
			case LEDSegment7.SA | LEDSegment7.SC | LEDSegment7.SD:
			case LEDSegment7.SB | LEDSegment7.SC | LEDSegment7.SD:
			case LEDSegment7.SA | LEDSegment7.SB | LEDSegment7.SC | LEDSegment7.SD:
			case LEDSegment7.SE:
			{
				LEDSegment7 num2 = segment - 1;
				if (num2 <= (LEDSegment7.SA | LEDSegment7.SB))
				{
					switch (num2)
					{
					case LEDSegment7.Empty:
					{
						graphicsPath.AddPolygon(GetSegment7(p, s));
						graphicsPath.CloseAllFigures();
						using (Matrix matrix6 = new Matrix())
						{
							matrix6.Translate(0f, (0f - sizeF.Height) / 2f + num / 2f);
							graphicsPath.Transform(ToMatrix3x2(matrix6));
							return graphicsPath;
						}
					}
					case LEDSegment7.SA | LEDSegment7.SB:
					{
						graphicsPath.AddPolygon(GetSegment7(p, s2));
						graphicsPath.CloseAllFigures();
						using (Matrix matrix5 = new Matrix())
						{
							matrix5.RotateAt(90f, p);
							graphicsPath.Transform(ToMatrix3x2(matrix5));
							matrix5.Reset();
							matrix5.Translate(sizeF.Width / 2f - num / 2f, sizeF.Height / 4f - num / 4f);
							graphicsPath.Transform(ToMatrix3x2(matrix5));
							return graphicsPath;
						}
					}
					case LEDSegment7.SA:
					{
						graphicsPath.AddPolygon(GetSegment7(p, s2));
						graphicsPath.CloseAllFigures();
						using (Matrix matrix4 = new Matrix())
						{
							matrix4.RotateAt(90f, p);
							graphicsPath.Transform(ToMatrix3x2(matrix4));
							matrix4.Reset();
							matrix4.Translate(sizeF.Width / 2f - num / 2f, (0f - sizeF.Height) / 4f + num / 4f);
							graphicsPath.Transform(ToMatrix3x2(matrix4));
							return graphicsPath;
						}
					}
					case LEDSegment7.SB:
						goto end_IL_007f;
					}
				}
				switch (segment)
				{
				case LEDSegment7.SD:
				{
					graphicsPath.AddPolygon(GetSegment7(p, s));
					graphicsPath.CloseAllFigures();
					using (Matrix matrix8 = new Matrix())
					{
						matrix8.RotateAt(180f, p);
						graphicsPath.Transform(ToMatrix3x2(matrix8));
						matrix8.Reset();
						matrix8.Translate(0f, sizeF.Height / 2f - num / 2f);
						graphicsPath.Transform(ToMatrix3x2(matrix8));
						return graphicsPath;
					}
				}
				case LEDSegment7.SE:
				{
					graphicsPath.AddPolygon(GetSegment7(p, s2));
					graphicsPath.CloseAllFigures();
					using (Matrix matrix7 = new Matrix())
					{
						matrix7.RotateAt(90f, p);
						graphicsPath.Transform(ToMatrix3x2(matrix7));
						matrix7.Reset();
						matrix7.Translate((0f - sizeF.Width) / 2f + num / 2f, sizeF.Height / 4f - num / 4f);
						graphicsPath.Transform(ToMatrix3x2(matrix7));
						return graphicsPath;
					}
				}
				}
				break;
			}
			case LEDSegment7.SG:
				graphicsPath.AddPolygon(GetSegment7(p, s));
				graphicsPath.CloseAllFigures();
				break;
			case LEDSegment7.SF:
			{
				graphicsPath.AddPolygon(GetSegment7(p, s2));
				graphicsPath.CloseAllFigures();
				using (Matrix matrix3 = new Matrix())
				{
					matrix3.RotateAt(90f, p);
					graphicsPath.Transform(ToMatrix3x2(matrix3));
					matrix3.Reset();
					matrix3.Translate((0f - sizeF.Width) / 2f + num / 2f, (0f - sizeF.Height) / 4f + num / 4f);
					graphicsPath.Transform(ToMatrix3x2(matrix3));
					return graphicsPath;
				}
			}
			case LEDSegment7.SDP:
			{
				graphicsPath.AddEllipse(p.X, p.Y, num * 2f, num * 2f);
				using (Matrix matrix2 = new Matrix())
				{
					matrix2.Translate(sizeF.Width / 2f + num / 2f, sizeF.Height / 2f - num * 2f);
					graphicsPath.Transform(ToMatrix3x2(matrix2));
					return graphicsPath;
				}
			}
			case LEDSegment7.SComma:
				{
					graphicsPath.AddRectangle(new RectangleF(p.X, p.Y, num * 2f, num * 4f));
					using (Matrix matrix = new Matrix())
					{
						matrix.Translate(sizeF.Width / 2f + num / 2f, sizeF.Height / 2f - num * 2f);
						graphicsPath.Transform(ToMatrix3x2(matrix));
						return graphicsPath;
					}
				}
				end_IL_007f:
				break;
			}
			return graphicsPath;
		}

		private static IGraphicsPath GetSegment14(IGaugeDrawingResourceFactory resourceFactory, LEDSegment14 segment, PointF p, float size)
		{
			IGraphicsPath graphicsPath = resourceFactory.CreatePath();
			SizeF sizeF = new SizeF(size * 0.618034f, size);
			float num = sizeF.Width * 0.142857149f;
			SizeF s = new SizeF(sizeF.Width - num, num);
			SizeF s2 = new SizeF(sizeF.Height / 2f - num / 2f, num);
			s.Width -= num / 3f;
			s2.Width -= num / 3f;
			switch (segment)
			{
			case LEDSegment14.SG1:
			{
				s.Width = s.Width / 2f - num / 6f;
				graphicsPath.AddPolygon(GetSegment7(p, s));
				graphicsPath.CloseAllFigures();
				using (Matrix matrix8 = new Matrix())
				{
					matrix8.Translate((0f - s.Width) / 2f - num / 6f, 0f);
					graphicsPath.Transform(ToMatrix3x2(matrix8));
					return graphicsPath;
				}
			}
			case LEDSegment14.SG2:
			{
				s.Width = s.Width / 2f - num / 6f;
				graphicsPath.AddPolygon(GetSegment7(p, s));
				graphicsPath.CloseAllFigures();
				using (Matrix matrix7 = new Matrix())
				{
					matrix7.Translate(s.Width / 2f + num / 6f, 0f);
					graphicsPath.Transform(ToMatrix3x2(matrix7));
					return graphicsPath;
				}
			}
			case LEDSegment14.SJ:
			{
				s2.Width -= num / 3f;
				graphicsPath.AddPolygon(GetSegment7(p, s2));
				graphicsPath.CloseAllFigures();
				using (Matrix matrix6 = new Matrix())
				{
					matrix6.RotateAt(90f, p);
					graphicsPath.Transform(ToMatrix3x2(matrix6));
					matrix6.Reset();
					matrix6.Translate(0f, (0f - s2.Width) / 2f - num / 6f);
					graphicsPath.Transform(ToMatrix3x2(matrix6));
					return graphicsPath;
				}
			}
			case LEDSegment14.SM:
			{
				s2.Width -= num / 3f;
				graphicsPath.AddPolygon(GetSegment7(p, s2));
				graphicsPath.CloseAllFigures();
				using (Matrix matrix5 = new Matrix())
				{
					matrix5.RotateAt(90f, p);
					graphicsPath.Transform(ToMatrix3x2(matrix5));
					matrix5.Reset();
					matrix5.Translate(0f, s2.Width / 2f + num / 6f);
					graphicsPath.Transform(ToMatrix3x2(matrix5));
					return graphicsPath;
				}
			}
			case LEDSegment14.SH:
			{
				graphicsPath.AddPolygon(GetSegmentHKLN(resourceFactory, p, s, num, left: true));
				using (Matrix matrix4 = new Matrix())
				{
					matrix4.Translate((0f - (s.Width / 2f + num / 6f)) / 2f, (0f - s2.Width) / 2f - num / 6f);
					graphicsPath.Transform(ToMatrix3x2(matrix4));
					return graphicsPath;
				}
			}
			case LEDSegment14.SL:
			{
				graphicsPath.AddPolygon(GetSegmentHKLN(resourceFactory, p, s, num, left: true));
				using (Matrix matrix3 = new Matrix())
				{
					matrix3.Translate((s.Width / 2f + num / 6f) / 2f, s2.Width / 2f - num / 6f);
					graphicsPath.Transform(ToMatrix3x2(matrix3));
					return graphicsPath;
				}
			}
			case LEDSegment14.SK:
			{
				graphicsPath.AddPolygon(GetSegmentHKLN(resourceFactory, p, s, num, left: false));
				using (Matrix matrix2 = new Matrix())
				{
					matrix2.Translate((s.Width / 2f + num / 6f) / 2f, (0f - s2.Width) / 2f - num / 6f);
					graphicsPath.Transform(ToMatrix3x2(matrix2));
					return graphicsPath;
				}
			}
			case LEDSegment14.SN:
			{
				graphicsPath.AddPolygon(GetSegmentHKLN(resourceFactory, p, s, num, left: false));
				using (Matrix matrix = new Matrix())
				{
					matrix.Translate((0f - (s.Width / 2f + num / 6f)) / 2f, s2.Width / 2f - num / 6f);
					graphicsPath.Transform(ToMatrix3x2(matrix));
					return graphicsPath;
				}
			}
			default:
				return graphicsPath;
			}
		}

		internal static IGraphicsPath GetSegments(IGaugeDrawingResourceFactory resourceFactory, LEDSegment7 segments, PointF point, float size)
		{
			IGraphicsPath graphicsPath = resourceFactory.CreatePath();
			PointF p = new PointF(0f, 0f);
			if ((segments & LEDSegment7.SA) == LEDSegment7.SA)
			{
				using (IGraphicsPath graphicsPath2 = GetSegment7(resourceFactory, LEDSegment7.SA, p, size))
				{
					if (graphicsPath2.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath2, connect: false);
					}
				}
			}
			if ((segments & LEDSegment7.SB) == LEDSegment7.SB)
			{
				using (IGraphicsPath graphicsPath3 = GetSegment7(resourceFactory, LEDSegment7.SB, p, size))
				{
					if (graphicsPath3.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath3, connect: false);
					}
				}
			}
			if ((segments & LEDSegment7.SC) == LEDSegment7.SC)
			{
				using (IGraphicsPath graphicsPath4 = GetSegment7(resourceFactory, LEDSegment7.SC, p, size))
				{
					if (graphicsPath4.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath4, connect: false);
					}
				}
			}
			if ((segments & LEDSegment7.SD) == LEDSegment7.SD)
			{
				using (IGraphicsPath graphicsPath5 = GetSegment7(resourceFactory, LEDSegment7.SD, p, size))
				{
					if (graphicsPath5.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath5, connect: false);
					}
				}
			}
			if ((segments & LEDSegment7.SE) == LEDSegment7.SE)
			{
				using (IGraphicsPath graphicsPath6 = GetSegment7(resourceFactory, LEDSegment7.SE, p, size))
				{
					if (graphicsPath6.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath6, connect: false);
					}
				}
			}
			if ((segments & LEDSegment7.SF) == LEDSegment7.SF)
			{
				using (IGraphicsPath graphicsPath7 = GetSegment7(resourceFactory, LEDSegment7.SF, p, size))
				{
					if (graphicsPath7.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath7, connect: false);
					}
				}
			}
			if ((segments & LEDSegment7.SG) == LEDSegment7.SG)
			{
				using (IGraphicsPath graphicsPath8 = GetSegment7(resourceFactory, LEDSegment7.SG, p, size))
				{
					if (graphicsPath8.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath8, connect: false);
					}
				}
			}
			if ((segments & LEDSegment7.SDP) == LEDSegment7.SDP)
			{
				using (IGraphicsPath graphicsPath9 = GetSegment7(resourceFactory, LEDSegment7.SDP, p, size))
				{
					if (graphicsPath9.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath9, connect: false);
					}
				}
			}
			if ((segments & LEDSegment7.SComma) == LEDSegment7.SComma)
			{
				using (IGraphicsPath graphicsPath10 = GetSegment7(resourceFactory, LEDSegment7.SComma, p, size))
				{
					if (graphicsPath10.PointCount <= 0)
					{
						return graphicsPath;
					}
					graphicsPath.AddPath(graphicsPath10, connect: false);
					return graphicsPath;
				}
			}
			return graphicsPath;
		}

		internal static IGraphicsPath GetOrientedSegments(IGaugeDrawingResourceFactory resourceFactory, LEDSegment7 segments, PointF point, float size, SegmentsCache cache)
		{
			IGraphicsPath graphicsPath = cache.GetSegment(resourceFactory, segments, point, size);
			if (graphicsPath == null)
			{
				graphicsPath = GetSegments(resourceFactory, segments, point, size);
				using (Matrix matrix = new Matrix())
				{
					matrix.Shear(-0.0618034f, 0f);
					graphicsPath.Transform(ToMatrix3x2(matrix));
					matrix.Reset();
					matrix.Translate(point.X, point.Y);
					graphicsPath.Transform(ToMatrix3x2(matrix));
					matrix.Reset();
				}
				cache.SetSegment(resourceFactory, segments, graphicsPath, point, size);
			}
			return graphicsPath;
		}

		internal static IGraphicsPath GetSegments(IGaugeDrawingResourceFactory resourceFactory, LEDSegment14 segments, PointF point, float size)
		{
			IGraphicsPath graphicsPath = resourceFactory.CreatePath();
			PointF p = new PointF(0f, 0f);
			if ((segments & LEDSegment14.SA) == LEDSegment14.SA)
			{
				using (IGraphicsPath graphicsPath2 = GetSegment7(resourceFactory, LEDSegment7.SA, p, size))
				{
					if (graphicsPath2.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath2, connect: false);
					}
				}
			}
			if ((segments & LEDSegment14.SB) == LEDSegment14.SB)
			{
				using (IGraphicsPath graphicsPath3 = GetSegment7(resourceFactory, LEDSegment7.SB, p, size))
				{
					if (graphicsPath3.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath3, connect: false);
					}
				}
			}
			if ((segments & LEDSegment14.SC) == LEDSegment14.SC)
			{
				using (IGraphicsPath graphicsPath4 = GetSegment7(resourceFactory, LEDSegment7.SC, p, size))
				{
					if (graphicsPath4.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath4, connect: false);
					}
				}
			}
			if ((segments & LEDSegment14.SD) == LEDSegment14.SD)
			{
				using (IGraphicsPath graphicsPath5 = GetSegment7(resourceFactory, LEDSegment7.SD, p, size))
				{
					if (graphicsPath5.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath5, connect: false);
					}
				}
			}
			if ((segments & LEDSegment14.SE) == LEDSegment14.SE)
			{
				using (IGraphicsPath graphicsPath6 = GetSegment7(resourceFactory, LEDSegment7.SE, p, size))
				{
					if (graphicsPath6.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath6, connect: false);
					}
				}
			}
			if ((segments & LEDSegment14.SF) == LEDSegment14.SF)
			{
				using (IGraphicsPath graphicsPath7 = GetSegment7(resourceFactory, LEDSegment7.SF, p, size))
				{
					if (graphicsPath7.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath7, connect: false);
					}
				}
			}
			if ((segments & LEDSegment14.SDP) == LEDSegment14.SDP)
			{
				using (IGraphicsPath graphicsPath8 = GetSegment7(resourceFactory, LEDSegment7.SDP, p, size))
				{
					if (graphicsPath8.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath8, connect: false);
					}
				}
			}
			if ((segments & LEDSegment14.SComma) == LEDSegment14.SComma)
			{
				using (IGraphicsPath graphicsPath9 = GetSegment7(resourceFactory, LEDSegment7.SComma, p, size))
				{
					if (graphicsPath9.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath9, connect: false);
					}
				}
			}
			if ((segments & LEDSegment14.SG1) == LEDSegment14.SG1)
			{
				using (IGraphicsPath graphicsPath10 = GetSegment14(resourceFactory, LEDSegment14.SG1, p, size))
				{
					if (graphicsPath10.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath10, connect: false);
					}
				}
			}
			if ((segments & LEDSegment14.SG2) == LEDSegment14.SG2)
			{
				using (IGraphicsPath graphicsPath11 = GetSegment14(resourceFactory, LEDSegment14.SG2, p, size))
				{
					if (graphicsPath11.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath11, connect: false);
					}
				}
			}
			if ((segments & LEDSegment14.SJ) == LEDSegment14.SJ)
			{
				using (IGraphicsPath graphicsPath12 = GetSegment14(resourceFactory, LEDSegment14.SJ, p, size))
				{
					if (graphicsPath12.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath12, connect: false);
					}
				}
			}
			if ((segments & LEDSegment14.SM) == LEDSegment14.SM)
			{
				using (IGraphicsPath graphicsPath13 = GetSegment14(resourceFactory, LEDSegment14.SM, p, size))
				{
					if (graphicsPath13.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath13, connect: false);
					}
				}
			}
			if ((segments & LEDSegment14.SH) == LEDSegment14.SH)
			{
				using (IGraphicsPath graphicsPath14 = GetSegment14(resourceFactory, LEDSegment14.SH, p, size))
				{
					if (graphicsPath14.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath14, connect: false);
					}
				}
			}
			if ((segments & LEDSegment14.SK) == LEDSegment14.SK)
			{
				using (IGraphicsPath graphicsPath15 = GetSegment14(resourceFactory, LEDSegment14.SK, p, size))
				{
					if (graphicsPath15.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath15, connect: false);
					}
				}
			}
			if ((segments & LEDSegment14.SL) == LEDSegment14.SL)
			{
				using (IGraphicsPath graphicsPath16 = GetSegment14(resourceFactory, LEDSegment14.SL, p, size))
				{
					if (graphicsPath16.PointCount > 0)
					{
						graphicsPath.AddPath(graphicsPath16, connect: false);
					}
				}
			}
			if ((segments & LEDSegment14.SN) == LEDSegment14.SN)
			{
				using (IGraphicsPath graphicsPath17 = GetSegment14(resourceFactory, LEDSegment14.SN, p, size))
				{
					if (graphicsPath17.PointCount <= 0)
					{
						return graphicsPath;
					}
					graphicsPath.AddPath(graphicsPath17, connect: false);
					return graphicsPath;
				}
			}
			return graphicsPath;
		}

		internal static IGraphicsPath GetOrientedSegments(IGaugeDrawingResourceFactory resourceFactory, LEDSegment14 segments, PointF point, float size, SegmentsCache cache)
		{
			IGraphicsPath graphicsPath = cache.GetSegment(resourceFactory, segments, point, size);
			if (graphicsPath == null)
			{
				graphicsPath = GetSegments(resourceFactory, segments, point, size);
				using (Matrix matrix = new Matrix())
				{
					matrix.Shear(-0.0618034f, 0f);
					graphicsPath.Transform(ToMatrix3x2(matrix));
					matrix.Reset();
					matrix.Translate(point.X, point.Y);
					graphicsPath.Transform(ToMatrix3x2(matrix));
					matrix.Reset();
				}
				cache.SetSegment(resourceFactory, segments, graphicsPath, point, size);
			}
			return graphicsPath;
		}

		internal static IGraphicsPath GetSymbol7(IGaugeDrawingResourceFactory resourceFactory, char symbol, PointF point, float size, bool decDot, bool comma, bool sepDots, SegmentsCache cache)
		{
			LEDSegment7 lEDSegment = LEDSegment7.Empty;
			if (char.IsDigit(symbol))
			{
				lEDSegment = (LEDSegment7)Enum.Parse(typeof(LEDSegment7), "N" + symbol);
			}
			else
			{
				switch (symbol)
				{
				case '-':
					lEDSegment = LEDSegment7.SG;
					break;
				case ' ':
					lEDSegment = LEDSegment7.Empty;
					break;
				case '+':
					lEDSegment = LEDSegment7.Empty;
					break;
				default:
					try
					{
						lEDSegment = (LEDSegment7)Enum.Parse(typeof(LEDSegment7), "C" + symbol);
					}
					catch
					{
						lEDSegment = LEDSegment7.Unknown;
					}
					break;
				}
			}
			if (decDot)
			{
				lEDSegment |= LEDSegment7.SDP;
			}
			if (comma)
			{
				lEDSegment |= LEDSegment7.SComma;
			}
			return GetOrientedSegments(resourceFactory, lEDSegment, point, size, cache);
		}

		internal static IGraphicsPath GetSymbol14(IGaugeDrawingResourceFactory resourceFactory, char symbol, PointF point, float size, bool decDot, bool comma, bool sepDots, SegmentsCache cache)
		{
			LEDSegment14 lEDSegment = LEDSegment14.Empty;
			if (char.IsDigit(symbol))
			{
				lEDSegment = (LEDSegment14)Enum.Parse(typeof(LEDSegment14), "N" + symbol);
			}
			else
			{
				switch (symbol)
				{
				case ' ':
					lEDSegment = LEDSegment14.Empty;
					break;
				case '-':
					lEDSegment = LEDSegment14.SG;
					break;
				case '+':
					lEDSegment = LEDSegment14.Plus;
					break;
				case '$':
					lEDSegment = LEDSegment14.CDollar;
					break;
				default:
					try
					{
						lEDSegment = (LEDSegment14)Enum.Parse(typeof(LEDSegment14), "C" + symbol);
					}
					catch
					{
						lEDSegment = LEDSegment14.Unknown;
					}
					break;
				}
			}
			if (decDot)
			{
				lEDSegment |= LEDSegment14.SDP;
			}
			if (comma)
			{
				lEDSegment |= LEDSegment14.SComma;
			}
			return GetOrientedSegments(resourceFactory, lEDSegment, point, size, cache);
		}
	}
}
