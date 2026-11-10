using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Numerics;
using Reporting.Chart.WebForms.Rendering;
using Reporting.Rendering;
using Reporting.Chart.WebForms.Rendering.Gdi;

namespace Reporting.Chart.WebForms
{
	internal class GdiGraphics : IChartRenderingEngine
	{
		private Graphics graphics;

		public Matrix Transform
		{
			get
			{
				return graphics.Transform;
			}
			set
			{
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
			}
		}

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

		public bool IsClipEmpty => graphics.IsClipEmpty;

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

		public CompositingQuality CompositingQuality
		{
			get
			{
				return graphics.CompositingQuality;
			}
			set
			{
				graphics.CompositingQuality = value;
			}
		}

		public InterpolationMode InterpolationMode
		{
			get
			{
				return graphics.InterpolationMode;
			}
			set
			{
				graphics.InterpolationMode = value;
			}
		}

		public float GetDpiX() => graphics.DpiX;

		public void BindSurface(IRenderSurface surface) => graphics = ((GdiRenderSurface)surface).NativeGraphics;

		public void DrawLine(Pen pen, PointF pt1, PointF pt2)
		{
			graphics.DrawLine(pen, pt1, pt2);
		}

		public void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
		{
			graphics.DrawLine(pen, x1, y1, x2, y2);
		}

		public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr)
		{
			graphics.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttr);
		}

		public void DrawEllipse(Pen pen, float x, float y, float width, float height)
		{
			graphics.DrawEllipse(pen, x, y, width, height);
		}

		public void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension)
		{
			graphics.DrawCurve(pen, points, offset, numberOfSegments, tension);
		}

		public void DrawRectangle(Pen pen, int x, int y, int width, int height)
		{
			graphics.DrawRectangle(pen, x, y, width, height);
		}

		public void DrawPolygon(Pen pen, PointF[] points)
		{
			graphics.DrawPolygon(pen, points);
		}

		public void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format)
		{
			graphics.DrawString(s, font, brush, layoutRectangle, format);
		}

		public void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format)
		{
			graphics.DrawString(s, font, brush, point, format);
		}

		public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs)
		{
			graphics.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, imageAttrs);
		}

		public void DrawRectangle(Pen pen, float x, float y, float width, float height)
		{
			graphics.DrawRectangle(pen, x, y, width, height);
		}

		public void DrawPath(Pen pen, GraphicsPath path)
		{
			graphics.DrawPath(pen, path);
		}

		public void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			graphics.DrawPie(pen, x, y, width, height, startAngle, sweepAngle);
		}

		public void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			graphics.DrawArc(pen, x, y, width, height, startAngle, sweepAngle);
		}

		public void DrawImage(Image image, RectangleF rect)
		{
			graphics.DrawImage(image, rect);
		}

		public void DrawEllipse(Pen pen, RectangleF rect)
		{
			graphics.DrawEllipse(pen, rect);
		}

		public void DrawLines(Pen pen, PointF[] points)
		{
			graphics.DrawLines(pen, points);
		}

		public void FillEllipse(Brush brush, RectangleF rect)
		{
			graphics.FillEllipse(brush, rect);
		}

		public void FillPath(Brush brush, GraphicsPath path)
		{
			graphics.FillPath(brush, path);
		}

		public void FillRegion(Brush brush, Region region)
		{
			graphics.FillRegion(brush, region);
		}

		public void FillRectangle(Brush brush, RectangleF rect)
		{
			graphics.FillRectangle(brush, rect);
		}

		public void FillRectangle(Brush brush, float x, float y, float width, float height)
		{
			graphics.FillRectangle(brush, x, y, width, height);
		}

		public void FillPolygon(Brush brush, PointF[] points)
		{
			graphics.FillPolygon(brush, points);
		}

		public void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle)
		{
			graphics.FillPie(brush, x, y, width, height, startAngle, sweepAngle);
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat)
		{
			return graphics.MeasureString(text, font, layoutArea, stringFormat);
		}

		public SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat, out int charactersFitted, out int linesFilled)
		{
			return graphics.MeasureString(text, font, layoutArea, stringFormat, out charactersFitted, out linesFilled);
		}

		public SizeF MeasureString(string text, Font font)
		{
			return graphics.MeasureString(text, font);
		}

		public GraphicsState Save()
		{
			return graphics.Save();
		}

		public void Restore(GraphicsState gstate)
		{
			graphics.Restore(gstate);
		}

		public void ResetClip()
		{
			graphics.ResetClip();
		}

		public void SetClip(RectangleF rect)
		{
			graphics.SetClip(rect);
		}

		public void SetClip(GraphicsPath path, CombineMode combineMode)
		{
			graphics.SetClip(path, combineMode);
		}

		public void TranslateTransform(float dx, float dy)
		{
			graphics.TranslateTransform(dx, dy);
		}

		public void BeginSelection(string hRef, string title)
		{
		}

		public void EndSelection()
		{
		}

		// --- Milestone A3: backend-agnostic overloads (Rendering.* interfaces) ---
		// Unwrap the port interface to the concrete GDI+ object it wraps and
		// delegate to the existing method above — same call, same output.

		private static Pen Native(IPen pen) => ((GdiPen)pen).NativePen;

		private static Brush Native(IBrush brush) => brush switch
		{
			GdiSolidBrush b => b.NativeBrush,
			GdiLinearGradientBrush b => b.NativeBrush,
			GdiTextureBrush b => b.NativeBrush,
			GdiHatchBrush b => b.NativeBrush,
			GdiPathGradientBrush b => b.NativeBrush,
			_ => throw new NotSupportedException($"Unrecognized IBrush implementation: {brush.GetType()}"),
		};

		private static Font Native(IChartFont font) => ((GdiChartFont)font).NativeFont;

		private static StringFormat Native(ITextFormat format) => format == null ? null : ((GdiTextFormat)format).NativeFormat;

		private static GraphicsPath Native(IGraphicsPath path) => ((GdiGraphicsPath)path).NativePath;

		private static Region Native(IClipRegion region) => ((GdiClipRegion)region).NativeRegion;

		private static Image Native(IChartImage image) => ((GdiChartImage)image).NativeImage;

		private static ImageAttributes Native(IImageDrawOptions options) => ((GdiImageDrawOptions)options).NativeAttributes;

		public void DrawLine(IPen pen, PointF pt1, PointF pt2) => DrawLine(Native(pen), pt1, pt2);

		public void DrawLine(IPen pen, float x1, float y1, float x2, float y2) => DrawLine(Native(pen), x1, y1, x2, y2);

		public void DrawImage(IChartImage image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, IImageDrawOptions imageAttr) =>
			DrawImage(Native(image), destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, Native(imageAttr));

		public void DrawEllipse(IPen pen, float x, float y, float width, float height) => DrawEllipse(Native(pen), x, y, width, height);

		public void DrawCurve(IPen pen, PointF[] points, int offset, int numberOfSegments, float tension) => DrawCurve(Native(pen), points, offset, numberOfSegments, tension);

		public void DrawRectangle(IPen pen, int x, int y, int width, int height) => DrawRectangle(Native(pen), x, y, width, height);

		public void DrawPolygon(IPen pen, PointF[] points) => DrawPolygon(Native(pen), points);

		public void DrawString(string s, IChartFont font, IBrush brush, RectangleF layoutRectangle, ITextFormat format) =>
			DrawString(s, Native(font), Native(brush), layoutRectangle, Native(format));

		public void DrawString(string s, IChartFont font, IBrush brush, PointF point, ITextFormat format) =>
			DrawString(s, Native(font), Native(brush), point, Native(format));

		public void DrawImage(IChartImage image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, IImageDrawOptions imageAttrs) =>
			DrawImage(Native(image), destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, Native(imageAttrs));

		public void DrawRectangle(IPen pen, float x, float y, float width, float height) => DrawRectangle(Native(pen), x, y, width, height);

		public void DrawPath(IPen pen, IGraphicsPath path) => DrawPath(Native(pen), Native(path));

		public void DrawPie(IPen pen, float x, float y, float width, float height, float startAngle, float sweepAngle) => DrawPie(Native(pen), x, y, width, height, startAngle, sweepAngle);

		public void DrawArc(IPen pen, float x, float y, float width, float height, float startAngle, float sweepAngle) => DrawArc(Native(pen), x, y, width, height, startAngle, sweepAngle);

		public void DrawImage(IChartImage image, RectangleF rect) => DrawImage(Native(image), rect);

		public void DrawEllipse(IPen pen, RectangleF rect) => DrawEllipse(Native(pen), rect);

		public void DrawLines(IPen pen, PointF[] points) => DrawLines(Native(pen), points);

		public void FillEllipse(IBrush brush, RectangleF rect) => FillEllipse(Native(brush), rect);

		public void FillPath(IBrush brush, IGraphicsPath path) => FillPath(Native(brush), Native(path));

		public void FillRegion(IBrush brush, IClipRegion region) => FillRegion(Native(brush), Native(region));

		public void FillRectangle(IBrush brush, RectangleF rect) => FillRectangle(Native(brush), rect);

		public void FillRectangle(IBrush brush, float x, float y, float width, float height) => FillRectangle(Native(brush), x, y, width, height);

		public void FillPolygon(IBrush brush, PointF[] points) => FillPolygon(Native(brush), points);

		public void FillPie(IBrush brush, float x, float y, float width, float height, float startAngle, float sweepAngle) => FillPie(Native(brush), x, y, width, height, startAngle, sweepAngle);

		public SizeF MeasureString(string text, IChartFont font, SizeF layoutArea, ITextFormat stringFormat) => MeasureString(text, Native(font), layoutArea, Native(stringFormat));

		public SizeF MeasureString(string text, IChartFont font) => MeasureString(text, Native(font));

		public SizeF MeasureString(string text, IChartFont font, SizeF layoutArea, ITextFormat stringFormat, out int charactersFitted, out int linesFilled) =>
			MeasureString(text, Native(font), layoutArea, Native(stringFormat), out charactersFitted, out linesFilled);

		public void SetClip(IGraphicsPath path, CombineMode combineMode) => SetClip(Native(path), combineMode);

		public IClipRegion GetClipRegion() => new GdiClipRegion(Clip);

		public void SetClipRegion(IClipRegion region) => Clip = Native(region);

		public Matrix3x2 GetTransform()
		{
			var m = Transform;
			var elements = m.Elements;
			return new Matrix3x2(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]);
		}

		public void SetTransform(Matrix3x2 matrix)
		{
			Transform = new Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.M31, matrix.M32);
		}
	}
}
