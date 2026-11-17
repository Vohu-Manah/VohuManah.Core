using System.Drawing;
using System.Drawing.Drawing2D;

namespace Reporting.Rendering
{
	/// <summary>
	/// Stroke resource. Abstracts <see cref="System.Drawing.Pen"/>.
	/// Surface measured in the engine (Appendix A.4): created as <c>(color, width)</c>;
	/// dash/cap/join set post-construction. Members kept mutable to mirror GDI+ usage
	/// and minimise caller migration.
	/// </summary>
	internal interface IPen : IRenderingResource
	{
		Color Color { get; set; }

		float Width { get; set; }

		DashStyle DashStyle { get; set; }

		LineCap StartCap { get; set; }

		LineCap EndCap { get; set; }

		LineJoin LineJoin { get; set; }

		PenAlignment Alignment { get; set; }

		/// <summary>Abstracts <c>Pen.MiterLimit</c> — the maximum ratio between miter length and line width before a miter join is clipped to a bevel.</summary>
		float MiterLimit { get; set; }

		/// <summary>Abstracts <c>Pen.DashPattern</c> — a custom on/off dash sequence, used when <see cref="DashStyle"/> alone (e.g. GDI+'s built-in <c>Dot</c> spacing) isn't the desired cadence.</summary>
		float[] DashPattern { get; set; }

		/// <summary>
		/// Abstracts <c>Pen.CustomStartCap</c>/<c>CustomEndCap</c> — GDI+-only, no cross-platform
		/// primitive (the permanent C3 <c>AdjustableArrowCap</c> holdout documented in
		/// chart-gdi-type-abstraction.md). Real on Gdi backends; throws <see cref="System.NotSupportedException"/>
		/// on Skia, same as the unsupported gradient/hatch pen-brush sources.
		/// </summary>
		CustomLineCap CustomStartCap { get; set; }

		/// <summary>See <see cref="CustomStartCap"/>.</summary>
		CustomLineCap CustomEndCap { get; set; }

		/// <summary>Abstracts <c>Pen.Clone()</c> — returns an independent copy with the same property values.</summary>
		IPen Clone();
	}
}
