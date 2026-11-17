using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using Reporting.Rendering;
using Reporting.Gauge.WebForms.Rendering;

namespace Reporting.Gauge.WebForms
{
	internal interface IGaugeRenderingEngine
	{
		Matrix Transform
		{
			get;
			set;
		}

		SmoothingMode SmoothingMode
		{
			get;
			set;
		}

		TextRenderingHint TextRenderingHint
		{
			get;
			set;
		}

		Region Clip
		{
			get;
			set;
		}

		Graphics Graphics
		{
			get;
			set;
		}

		bool IsClipEmpty
		{
			get;
		}

		void DrawLine(Pen pen, PointF pt1, PointF pt2);

		void DrawLine(Pen pen, float x1, float y1, float x2, float y2);

		void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr);

		void DrawEllipse(Pen pen, float x, float y, float width, float height);

		void DrawCurve(Pen pen, PointF[] points, int offset, int numberOfSegments, float tension);

		void DrawRectangle(Pen pen, int x, int y, int width, int height);

		void DrawPolygon(Pen pen, PointF[] points);

		void DrawString(string s, Font font, Brush brush, RectangleF layoutRectangle, StringFormat format);

		void DrawString(string s, Font font, Brush brush, PointF point, StringFormat format);

		void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs);

		void DrawRectangle(Pen pen, float x, float y, float width, float height);

		void DrawPath(Pen pen, GraphicsPath path);

		void DrawPie(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle);

		void DrawArc(Pen pen, float x, float y, float width, float height, float startAngle, float sweepAngle);

		void DrawImage(Image image, RectangleF rect);

		void DrawEllipse(Pen pen, RectangleF rect);

		void DrawLines(Pen pen, PointF[] points);

		void FillEllipse(Brush brush, RectangleF rect);

		void FillPath(Brush brush, GraphicsPath path);

		void FillPath(Brush brush, GraphicsPath path, float angle, bool useBrushOffset, bool circularFill);

		void FillRegion(Brush brush, Region region);

		void FillRectangle(Brush brush, RectangleF rect);

		void FillRectangle(Brush brush, float x, float y, float width, float height);

		void FillPolygon(Brush brush, PointF[] points);

		void FillPie(Brush brush, float x, float y, float width, float height, float startAngle, float sweepAngle);

		SizeF MeasureString(string text, Font font, SizeF layoutArea, StringFormat stringFormat);

		SizeF MeasureString(string text, Font font);

		GraphicsState Save();

		void Restore(GraphicsState gstate);

		void ResetClip();

		void SetClip(RectangleF rect);

		void SetClip(GraphicsPath path, CombineMode combineMode);

		void TranslateTransform(float dx, float dy);

		void BeginSelection(string hRef, string title);

		void EndSelection();

		// --- Interface-typed overloads (Milestone A3 equivalent — see tasks/gauge-gdi-type-abstraction.md).
		// Mirror the GDI+-typed members above one-for-one; old signatures kept so callers migrate
		// incrementally, same approach as the Chart engine's IChartRenderingEngine.

		void DrawLine(IPen pen, PointF pt1, PointF pt2);

		void DrawLine(IPen pen, float x1, float y1, float x2, float y2);

		void DrawEllipse(IPen pen, float x, float y, float width, float height);

		void DrawEllipse(IPen pen, RectangleF rect);

		void DrawCurve(IPen pen, PointF[] points, int offset, int numberOfSegments, float tension);

		void DrawRectangle(IPen pen, int x, int y, int width, int height);

		void DrawRectangle(IPen pen, float x, float y, float width, float height);

		void DrawPolygon(IPen pen, PointF[] points);

		void DrawString(string s, IChartFont font, IBrush brush, RectangleF layoutRectangle, ITextFormat format);

		void DrawString(string s, IChartFont font, IBrush brush, PointF point, ITextFormat format);

		void DrawPath(IPen pen, IGraphicsPath path);

		void DrawPie(IPen pen, float x, float y, float width, float height, float startAngle, float sweepAngle);

		void DrawArc(IPen pen, float x, float y, float width, float height, float startAngle, float sweepAngle);

		void DrawLines(IPen pen, PointF[] points);

		void FillEllipse(IBrush brush, RectangleF rect);

		void FillPath(IBrush brush, IGraphicsPath path);

		void FillPath(IBrush brush, IGraphicsPath path, float angle, bool useBrushOffset, bool circularFill);

		void FillRectangle(IBrush brush, RectangleF rect);

		void FillRectangle(IBrush brush, float x, float y, float width, float height);

		void FillPolygon(IBrush brush, PointF[] points);

		void FillPie(IBrush brush, float x, float y, float width, float height, float startAngle, float sweepAngle);

		SizeF MeasureString(string text, IChartFont font, SizeF layoutArea, ITextFormat stringFormat);

		SizeF MeasureString(string text, IChartFont font);

		/// <summary>Interface-typed equivalent of the <see cref="Clip"/> property getter (Milestone A4 equivalent).</summary>
		IGaugeClipRegion GetClipRegion();

		/// <summary>Interface-typed equivalent of the <see cref="Clip"/> property setter (Milestone A4 equivalent).</summary>
		void SetClipRegion(IGaugeClipRegion region);

		/// <summary>Interface-typed sibling of <see cref="DrawImage(Image, Rectangle, int, int, int, int, GraphicsUnit, ImageAttributes)"/> (Milestone A4 equivalent — added alongside the clip-region work since both real callers need it).</summary>
		void DrawImage(IChartImage image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, IImageDrawOptions imageAttr);
	}
}
