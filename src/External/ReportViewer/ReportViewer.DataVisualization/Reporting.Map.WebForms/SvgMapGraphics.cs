using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using Reporting.Rendering;
using Reporting.Map.WebForms.Rendering.Gdi;

namespace Reporting.Map.WebForms
{
	internal class SvgMapGraphics : SvgRendering, IMapRenderingEngine
	{
		private Graphics graphics;

		public new Matrix Transform
		{
			get
			{
				return graphics.Transform;
			}
			set
			{
				mapMatrix = value;
				graphics.Transform = value;
			}
		}

		public SmoothingMode SmoothingMode
		{
			get
			{
				return graphics.SmoothingMode;
			}
			set
			{
				graphics.SmoothingMode = value;
				SetSmoothingMode(value == SmoothingMode.AntiAlias, shape: true);
			}
		}

		public bool IsClipEmpty => graphics.IsClipEmpty;

		public Region Clip
		{
			get
			{
				return graphics.Clip;
			}
			set
			{
				graphics.Clip = value;
			}
		}

		public Graphics Graphics
		{
			get
			{
				return graphics;
			}
			set
			{
				graphics = value;
			}
		}

		public TextRenderingHint TextRenderingHint
		{
			get
			{
				return graphics.TextRenderingHint;
			}
			set
			{
				graphics.TextRenderingHint = value;
				SetSmoothingMode(value == TextRenderingHint.AntiAlias || value == TextRenderingHint.SystemDefault || value == TextRenderingHint.ClearTypeGridFit, shape: false);
			}
		}

		public SvgMapGraphics(CommonElements common)
		{
		}

		public void DrawLine(Pen pen, PointF pt1, PointF pt2)
		{
			base.Pen = pen;
			DrawLine(pt1, pt2);
		}

		public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
		{
			DrawLine(pen, new PointF(x1, y1), new PointF(x2, y2));
		}

		public new void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
		{
			base.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr);
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			base.Pen = pen;
			DrawEllipse(new RectangleF(x, y, width, height));
		}

		public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
		{
			base.Pen = pen;
			DrawCurve(points, offset, numberOfSegments, tension);
		}

		public void DrawRectangle(Pen pen, int x, int y, int width, int height)
		{
			DrawRectangle(pen, (float)x, (float)y, (float)width, (float)height);
		}

		public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
		{
			mapFont = font;
			base.Brush = brush;
			mapStringFormat = format;
			DrawString(s, layoutRectangle);
		}

		public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
		{
			mapFont = font;
			base.Brush = brush;
			mapStringFormat = format;
			DrawString(s, point);
		}

		public new void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
		{
			base.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs);
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			base.Pen = pen;
			DrawRectangle(new RectangleF(x, y, width, height));
		}

		public void DrawPath(Pen pen, GraphicsPath path)
		{
			base.Pen = pen;
			DrawPath(path);
		}

		public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			base.Pen = pen;
			DrawPie(new RectangleF(x, y, width, height), startAngle, sweepAngle);
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			base.Pen = pen;
			DrawArc(new RectangleF(x, y, width, height), startAngle, sweepAngle);
		}

		public new void DrawImage(Image image, RectangleF rect)
		{
			base.DrawImage(image, rect);
		}

		public void DrawEllipse(Pen pen, RectangleF rect)
		{
			base.Pen = pen;
			DrawEllipse(rect);
		}

		public void DrawLines(Pen pen, PointF[] points)
		{
			base.Pen = pen;
			DrawLines(points);
		}

		public void FillEllipse(Brush brush, RectangleF rect)
		{
			base.Brush = brush;
			FillEllipse(rect);
		}

		public void FillPath(Brush brush, GraphicsPath path)
		{
			base.Brush = brush;
			FillPath(path);
		}

		public void FillPath(Brush brush, GraphicsPath path, float angle, bool useBrushOffset, bool circularFill)
		{
			base.Brush = brush;
			FillPath(path);
		}

		public void FillRegion(Brush brush, Region region)
		{
		}

		public void FillRectangle(Brush brush, RectangleF rect)
		{
			base.Brush = brush;
			if (brush is TextureBrush)
			{
				FillTexturedRectangle((TextureBrush)brush, rect);
			}
			else
			{
				FillRectangle(rect);
			}
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			base.Brush = brush;
			if (brush is TextureBrush)
			{
				FillTexturedRectangle((TextureBrush)brush, new RectangleF(x, y, width, height));
			}
			else
			{
				FillRectangle(new RectangleF(x, y, width, height));
			}
		}

		public void FillPolygon(Brush brush, PointF[] points)
		{
			base.Brush = brush;
			FillPolygon(points);
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			base.Brush = brush;
			FillPie(new RectangleF(x, y, width, height), startAngle, sweepAngle);
		}

		public new void SetClip(RectangleF rect)
		{
			base.SetClip(rect);
			graphics.SetClip(rect);
		}

		public new void ResetClip()
		{
			base.ResetClip();
			graphics.ResetClip();
		}

		public void SetClip(GraphicsPath path, CombineMode combineMode)
		{
		}

		public void TranslateTransform(float dx, float dy)
		{
			graphics.TranslateTransform(dx, dy);
		}

		public void SetGradient(Color firstColor, Color secondColor, GradientType gradientType)
		{
			mapSvgGradientType = (SvgGradientType)Enum.Parse(typeof(SvgGradientType), gradientType.ToString());
			mapBrushColor = firstColor;
			mapBrushSecondColor = secondColor;
		}

		public void DrawPolygon(Pen pen, PointF[] points)
		{
			base.Pen = pen;
			DrawPolygon(points);
		}

		public GraphicsState Save()
		{
			return graphics.Save();
		}

		public void Restore(GraphicsState gstate)
		{
			graphics.Restore(gstate);
			Transform = graphics.Transform;
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
		{
			return graphics.MeasureString(text, font, layoutArea, stringFormat);
		}

		public SizeF MeasureString(string text, Font font)
		{
			return graphics.MeasureString(text, font);
		}

		public void BeginSelection(string hRef, string title)
		{
			BeginSvgSelection(hRef, title);
		}

		public void EndSelection()
		{
			EndSvgSelection();
		}

		// --- Milestone A: backend-agnostic overloads (Rendering.* interfaces) ---
		// Unwrap the port interface to the concrete GDI+ object it wraps and
		// delegate to the existing method above — same call, same output.

		private static Pen Native(IPen pen) => ((GdiPen)pen).NativePen;

		private static Brush Native(IBrush brush) => brush switch
		{
			GdiSolidBrush b => b.NativeBrush,
			GdiLinearGradientBrush b => b.NativeBrush,
			GdiHatchBrush b => b.NativeBrush,
			GdiPathGradientBrush b => b.NativeBrush,
			_ => throw new NotSupportedException($"Unrecognized IBrush implementation: {brush.GetType()}"),
		};

		private static Font Native(IChartFont font) => ((GdiChartFont)font).NativeFont;

		private static StringFormat Native(ITextFormat format) => format == null ? null : ((GdiTextFormat)format).NativeFormat;

		private static GraphicsPath Native(IGraphicsPath path) => ((GdiGraphicsPath)path).NativePath;

		public void DrawLine(IPen pen, PointF pt1, PointF pt2) => DrawLine(Native(pen), pt1, pt2);

		public void DrawLine(IPen pen, float x1, float y1, float x2, float y2) => DrawLine(Native(pen), x1, y1, x2, y2);

		public void DrawEllipse(IPen pen, float x, float y, float width, float height) => DrawEllipse(Native(pen), x, y, width, height);

		public void DrawCurve(IPen pen, PointF[] points, int offset, int numberOfSegments, float tension) => DrawCurve(Native(pen), points, offset, numberOfSegments, tension);

		public void DrawRectangle(IPen pen, int x, int y, int width, int height) => DrawRectangle(Native(pen), x, y, width, height);

		public void DrawPolygon(IPen pen, PointF[] points) => DrawPolygon(Native(pen), points);

		public void DrawString(string s, IChartFont font, IBrush brush, RectangleF layoutRectangle, ITextFormat format) =>
			DrawString(s, Native(font), Native(brush), layoutRectangle, Native(format));

		public void DrawString(string s, IChartFont font, IBrush brush, PointF point, ITextFormat format) =>
			DrawString(s, Native(font), Native(brush), point, Native(format));

		public void DrawRectangle(IPen pen, float x, float y, float width, float height) => DrawRectangle(Native(pen), x, y, width, height);

		public void DrawPath(IPen pen, IGraphicsPath path) => DrawPath(Native(pen), Native(path));

		public void DrawPie(IPen pen, float x, float y, float width, float height, float startAngle, float sweepAngle) => DrawPie(Native(pen), x, y, width, height, startAngle, sweepAngle);

		public void DrawArc(IPen pen, float x, float y, float width, float height, float startAngle, float sweepAngle) => DrawArc(Native(pen), x, y, width, height, startAngle, sweepAngle);

		public void DrawEllipse(IPen pen, RectangleF rect) => DrawEllipse(Native(pen), rect);

		public void DrawLines(IPen pen, PointF[] points) => DrawLines(Native(pen), points);

		public void FillEllipse(IBrush brush, RectangleF rect) => FillEllipse(Native(brush), rect);

		public void FillPath(IBrush brush, IGraphicsPath path) => FillPath(Native(brush), Native(path));

		public void FillPath(IBrush brush, IGraphicsPath path, float angle, bool useBrushOffset, bool circularFill) => FillPath(Native(brush), Native(path), angle, useBrushOffset, circularFill);

		public void FillRectangle(IBrush brush, RectangleF rect) => FillRectangle(Native(brush), rect);

		public void FillRectangle(IBrush brush, float x, float y, float width, float height) => FillRectangle(Native(brush), x, y, width, height);

		public void FillPolygon(IBrush brush, PointF[] points) => FillPolygon(Native(brush), points);

		public void FillPie(IBrush brush, float x, float y, float width, float height, float startAngle, float sweepAngle) => FillPie(Native(brush), x, y, width, height, startAngle, sweepAngle);

		public SizeF MeasureString(string text, IChartFont font, SizeF layoutArea, ITextFormat stringFormat) => MeasureString(text, Native(font), layoutArea, Native(stringFormat));

		public SizeF MeasureString(string text, IChartFont font) => MeasureString(text, Native(font));

		public void SetClip(IGraphicsPath path, CombineMode combineMode) => SetClip(Native(path), combineMode);
	}
}
