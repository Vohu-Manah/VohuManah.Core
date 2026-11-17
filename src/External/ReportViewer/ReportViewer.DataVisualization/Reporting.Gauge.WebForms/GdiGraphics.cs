using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Reporting.Rendering;
using Reporting.Gauge.WebForms.Rendering;
using Reporting.Gauge.WebForms.Rendering.Gdi;

namespace Reporting.Gauge.WebForms
{
	internal class GdiGraphics : IGaugeRenderingEngine
	{
		private Graphics graphics;

		private static Pen Native(IPen pen) => ((GdiPen)pen).NativePen;

		private static Brush Native(IBrush brush) => brush switch
		{
			GdiSolidBrush b => b.NativeBrush,
			GdiLinearGradientBrush b => b.NativeBrush,
			GdiTextureBrush b => b.NativeBrush,
			GdiHatchBrush b => b.NativeBrush,
			GdiPathGradientBrush b => b.NativeBrush,
			_ => throw new System.NotSupportedException($"Unrecognized IBrush implementation: {brush.GetType()}"),
		};

		private static Font Native(IChartFont font) => ((GdiChartFont)font).NativeFont;

		private static StringFormat Native(ITextFormat format) => format == null ? null : ((GdiTextFormat)format).NativeFormat;

		private static GraphicsPath Native(IGraphicsPath path) => ((GdiGraphicsPath)path).NativePath;

		private static Region Native(IGaugeClipRegion region) => ((GdiClipRegion)region).NativeRegion;

		private static Image Native(IChartImage image) => ((GdiChartImage)image).NativeImage;

		private static ImageAttributes Native(IImageDrawOptions options) => ((GdiImageDrawOptions)options).NativeAttributes;

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

		public void FillPath(Brush brush, GraphicsPath path, float angle, bool useBrushOffset, bool circularFill)
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

		public void DrawLine(IPen pen, PointF pt1, PointF pt2) => graphics.DrawLine(Native(pen), pt1, pt2);

		public void DrawLine(IPen pen, float x1, float y1, float x2, float y2) => graphics.DrawLine(Native(pen), x1, y1, x2, y2);

		public void DrawEllipse(IPen pen, float x, float y, float width, float height) => graphics.DrawEllipse(Native(pen), x, y, width, height);

		public void DrawEllipse(IPen pen, RectangleF rect) => graphics.DrawEllipse(Native(pen), rect);

		public void DrawCurve(IPen pen, PointF[] points, int offset, int numberOfSegments, float tension) =>
			graphics.DrawCurve(Native(pen), points, offset, numberOfSegments, tension);

		public void DrawRectangle(IPen pen, int x, int y, int width, int height) => graphics.DrawRectangle(Native(pen), x, y, width, height);

		public void DrawRectangle(IPen pen, float x, float y, float width, float height) => graphics.DrawRectangle(Native(pen), x, y, width, height);

		public void DrawPolygon(IPen pen, PointF[] points) => graphics.DrawPolygon(Native(pen), points);

		public void DrawString(string s, IChartFont font, IBrush brush, RectangleF layoutRectangle, ITextFormat format) =>
			graphics.DrawString(s, Native(font), Native(brush), layoutRectangle, Native(format));

		public void DrawString(string s, IChartFont font, IBrush brush, PointF point, ITextFormat format) =>
			graphics.DrawString(s, Native(font), Native(brush), point, Native(format));

		public void DrawPath(IPen pen, IGraphicsPath path) => graphics.DrawPath(Native(pen), Native(path));

		public void DrawPie(IPen pen, float x, float y, float width, float height, float startAngle, float sweepAngle) =>
			graphics.DrawPie(Native(pen), x, y, width, height, startAngle, sweepAngle);

		public void DrawArc(IPen pen, float x, float y, float width, float height, float startAngle, float sweepAngle) =>
			graphics.DrawArc(Native(pen), x, y, width, height, startAngle, sweepAngle);

		public void DrawLines(IPen pen, PointF[] points) => graphics.DrawLines(Native(pen), points);

		public void FillEllipse(IBrush brush, RectangleF rect) => graphics.FillEllipse(Native(brush), rect);

		public void FillPath(IBrush brush, IGraphicsPath path) => graphics.FillPath(Native(brush), Native(path));

		public void FillPath(IBrush brush, IGraphicsPath path, float angle, bool useBrushOffset, bool circularFill) =>
			graphics.FillPath(Native(brush), Native(path));

		public void FillRectangle(IBrush brush, RectangleF rect) => graphics.FillRectangle(Native(brush), rect);

		public void FillRectangle(IBrush brush, float x, float y, float width, float height) => graphics.FillRectangle(Native(brush), x, y, width, height);

		public void FillPolygon(IBrush brush, PointF[] points) => graphics.FillPolygon(Native(brush), points);

		public void FillPie(IBrush brush, float x, float y, float width, float height, float startAngle, float sweepAngle) =>
			graphics.FillPie(Native(brush), x, y, width, height, startAngle, sweepAngle);

		public SizeF MeasureString(string text, IChartFont font, SizeF layoutArea, ITextFormat stringFormat) =>
			graphics.MeasureString(text, Native(font), layoutArea, Native(stringFormat));

		public SizeF MeasureString(string text, IChartFont font) => graphics.MeasureString(text, Native(font));

		public IGaugeClipRegion GetClipRegion() => new GdiClipRegion(graphics.Clip);

		public void SetClipRegion(IGaugeClipRegion region) => graphics.Clip = Native(region);

		public void DrawImage(IChartImage image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, IImageDrawOptions imageAttr) =>
			graphics.DrawImage(Native(image), destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, Native(imageAttr));
	}
}
