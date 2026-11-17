using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;

namespace Reporting.Rendering
{
	/// <summary>
	/// Mutable geometry builder. Abstracts <see cref="System.Drawing.Drawing2D.GraphicsPath"/>.
	/// Appendix A.1: the full method surface actually used by the chart engine
	/// (~24 methods + 3 properties) — enumerated here so the contract is complete
	/// with no hidden API. <c>Transform</c> takes <see cref="Matrix3x2"/> (see
	/// <c>ITransform</c> decision, Appendix A.6).
	/// </summary>
	internal interface IGraphicsPath : IRenderingResource
	{
		FillMode FillMode { get; set; }

		PointF[] PathPoints { get; }

		byte[] PathTypes { get; }

		int PointCount { get; }

		void AddLine(PointF pt1, PointF pt2);

		void AddLine(float x1, float y1, float x2, float y2);

		void AddLines(PointF[] points);

		void AddArc(float x, float y, float width, float height, float startAngle, float sweepAngle);

		void AddBezier(PointF pt1, PointF pt2, PointF pt3, PointF pt4);

		/// <summary>
		/// Abstracts <c>GraphicsPath.AddBeziers(PointF[])</c> — a chained sequence of cubic Béziers
		/// sharing endpoints (distinct from the single 4-point <see cref="AddBezier"/>). Needed by
		/// the Gauge engine's <c>XamlRenderer</c> StreamGeometry parser (see
		/// tasks/gauge-gdi-type-abstraction.md item 2) — not in the original Appendix A.1 inventory.
		/// </summary>
		void AddBeziers(PointF[] points);

		void AddCurve(PointF[] points, float tension);

		void AddCurve(PointF[] points, int offset, int numberOfSegments, float tension);

		void AddClosedCurve(PointF[] points);

		void AddClosedCurve(PointF[] points, float tension);

		void AddEllipse(float x, float y, float width, float height);

		void AddEllipse(RectangleF rect);

		void AddRectangle(RectangleF rect);

		void AddPolygon(PointF[] points);

		void AddPie(float x, float y, float width, float height, float startAngle, float sweepAngle);

		void AddPath(IGraphicsPath addingPath, bool connect);

		void AddString(string text, IChartFont font, PointF origin, ITextFormat format);

		void AddString(string text, IChartFont font, RectangleF layoutRect, ITextFormat format);

		void StartFigure();

		void CloseFigure();

		void CloseAllFigures();

		void Flatten();

		/// <summary>Abstracts <c>GraphicsPath.Flatten(null, flatness)</c> — flattens with an explicit tolerance instead of the default.</summary>
		void Flatten(float flatness);

		void Widen(IPen pen);

		void Reverse();

		void SetMarkers();

		void Transform(Matrix3x2 matrix);

		RectangleF GetBounds();

		/// <summary>Abstracts <c>GraphicsPath.GetBounds(Matrix)</c> — the path's bounding box after applying <paramref name="matrix"/>, without mutating the path itself.</summary>
		RectangleF GetBounds(Matrix3x2 matrix);

		bool IsVisible(PointF point);

		void Reset();
	}
}
