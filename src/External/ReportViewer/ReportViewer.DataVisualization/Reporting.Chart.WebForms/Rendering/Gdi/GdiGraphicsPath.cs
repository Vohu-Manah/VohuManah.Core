using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using Reporting.Rendering;

namespace Reporting.Chart.WebForms.Rendering.Gdi
{
	/// <summary>Milestone A2 adapter — wraps <see cref="System.Drawing.Drawing2D.GraphicsPath"/> behind <see cref="IGraphicsPath"/>.</summary>
	internal sealed class GdiGraphicsPath : IGraphicsPath
	{
		internal GraphicsPath NativePath { get; }

		internal GdiGraphicsPath()
		{
			NativePath = new GraphicsPath();
		}

		internal GdiGraphicsPath(PointF[] points, byte[] types)
		{
			NativePath = new GraphicsPath(points, types);
		}

		public FillMode FillMode
		{
			get => NativePath.FillMode;
			set => NativePath.FillMode = value;
		}

		public PointF[] PathPoints => NativePath.PathPoints;

		public byte[] PathTypes => NativePath.PathTypes;

		public int PointCount => NativePath.PointCount;

		public void AddLine(PointF pt1, PointF pt2) => NativePath.AddLine(pt1, pt2);

		public void AddLine(float x1, float y1, float x2, float y2) => NativePath.AddLine(x1, y1, x2, y2);

		public void AddLines(PointF[] points) => NativePath.AddLines(points);

		public void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle) =>
			NativePath.AddArc(x, y, width, height, startAngle, sweepAngle);

		public void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4) => NativePath.AddBezier(pt1, pt2, pt3, pt4);

		public void AddBeziers(PointF[] points) => NativePath.AddBeziers(points);

		public void AddCurve(PointF[] points, float tension) => NativePath.AddCurve(points, tension);

		public void AddCurve(PointF[] points, int offset, int numberOfSegments, float tension) => NativePath.AddCurve(points, offset, numberOfSegments, tension);

		public void AddClosedCurve(PointF[] points) => NativePath.AddClosedCurve(points);

		public void AddClosedCurve(PointF[] points, float tension) => NativePath.AddClosedCurve(points, tension);

		public void AddEllipse(float x, float y, float width, float height) => NativePath.AddEllipse(x, y, width, height);

		public void AddEllipse(RectangleF rect) => NativePath.AddEllipse(rect);

		public void AddRectangle(RectangleF rect) => NativePath.AddRectangle(rect);

		public void AddPolygon(PointF[] points) => NativePath.AddPolygon(points);

		public void AddPie(float x, float y, float width, float height, float startAngle, float sweepAngle) =>
			NativePath.AddPie(x, y, width, height, startAngle, sweepAngle);

		public void AddPath(IGraphicsPath addingPath, bool connect) => NativePath.AddPath(((GdiGraphicsPath)addingPath).NativePath, connect);

		public void AddString(string text, IChartFont font, PointF origin, ITextFormat format)
		{
			var nativeFont = ((GdiChartFont)font).NativeFont;
			var nativeFormat = format == null ? null : ((GdiTextFormat)format).NativeFormat;
			NativePath.AddString(text, nativeFont.FontFamily, (int)nativeFont.Style, nativeFont.Size, origin, nativeFormat);
		}

		public void AddString(string text, IChartFont font, RectangleF layoutRect, ITextFormat format)
		{
			var nativeFont = ((GdiChartFont)font).NativeFont;
			var nativeFormat = format == null ? null : ((GdiTextFormat)format).NativeFormat;
			NativePath.AddString(text, nativeFont.FontFamily, (int)nativeFont.Style, nativeFont.Size, layoutRect, nativeFormat);
		}

		public void StartFigure() => NativePath.StartFigure();

		public void CloseFigure() => NativePath.CloseFigure();

		public void CloseAllFigures() => NativePath.CloseAllFigures();

		public void Flatten() => NativePath.Flatten();

		public void Flatten(float flatness) => NativePath.Flatten(null, flatness);

		public void Widen(IPen pen) => NativePath.Widen(((GdiPen)pen).NativePen);

		public void Reverse() => NativePath.Reverse();

		public void SetMarkers() => NativePath.SetMarkers();

		public void Transform(Matrix3x2 matrix)
		{
			using var nativeMatrix = new Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.M31, matrix.M32);
			NativePath.Transform(nativeMatrix);
		}

		public RectangleF GetBounds() => NativePath.GetBounds();

		public RectangleF GetBounds(Matrix3x2 matrix)
		{
			using var nativeMatrix = new Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.M31, matrix.M32);
			return NativePath.GetBounds(nativeMatrix);
		}

		public bool IsVisible(PointF point) => NativePath.IsVisible(point);

		public void Reset() => NativePath.Reset();

		public void Dispose() => NativePath.Dispose();
	}
}
