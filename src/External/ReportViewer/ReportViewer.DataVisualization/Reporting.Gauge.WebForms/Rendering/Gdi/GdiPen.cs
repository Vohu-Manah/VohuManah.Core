using System.Drawing;
using System.Drawing.Drawing2D;
using Reporting.Rendering;

namespace Reporting.Gauge.WebForms.Rendering.Gdi
{
	/// <summary>Milestone A2 adapter — wraps <see cref="System.Drawing.Pen"/> behind <see cref="IPen"/>.</summary>
	internal sealed class GdiPen : IPen
	{
		internal Pen NativePen { get; }

		internal GdiPen(Color color, float width)
		{
			NativePen = new Pen(color, width);
		}

		internal GdiPen(Brush brush, float width)
		{
			NativePen = new Pen(brush, width);
		}

		public Color Color
		{
			get => NativePen.Color;
			set => NativePen.Color = value;
		}

		public float Width
		{
			get => NativePen.Width;
			set => NativePen.Width = value;
		}

		public DashStyle DashStyle
		{
			get => NativePen.DashStyle;
			set => NativePen.DashStyle = value;
		}

		public LineCap StartCap
		{
			get => NativePen.StartCap;
			set => NativePen.StartCap = value;
		}

		public LineCap EndCap
		{
			get => NativePen.EndCap;
			set => NativePen.EndCap = value;
		}

		public LineJoin LineJoin
		{
			get => NativePen.LineJoin;
			set => NativePen.LineJoin = value;
		}

		public PenAlignment Alignment
		{
			get => NativePen.Alignment;
			set => NativePen.Alignment = value;
		}

		public float MiterLimit
		{
			get => NativePen.MiterLimit;
			set => NativePen.MiterLimit = value;
		}

		public float[] DashPattern
		{
			get => NativePen.DashPattern;
			set => NativePen.DashPattern = value;
		}

		public CustomLineCap CustomStartCap
		{
			get => NativePen.CustomStartCap;
			set => NativePen.CustomStartCap = value;
		}

		public CustomLineCap CustomEndCap
		{
			get => NativePen.CustomEndCap;
			set => NativePen.CustomEndCap = value;
		}

		public IPen Clone()
		{
			GdiPen gdiPen = new GdiPen(Color, Width);
			gdiPen.DashStyle = DashStyle;
			gdiPen.StartCap = StartCap;
			gdiPen.EndCap = EndCap;
			gdiPen.LineJoin = LineJoin;
			gdiPen.Alignment = Alignment;
			return gdiPen;
		}

		public void Dispose() => NativePen.Dispose();
	}
}
