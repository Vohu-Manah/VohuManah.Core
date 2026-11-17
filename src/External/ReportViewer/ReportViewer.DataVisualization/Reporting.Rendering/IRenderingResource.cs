using System;

namespace Reporting.Rendering
{
	// Milestone A1 skeleton — see tasks/chart-gdi-type-abstraction.md.
	// Backend-agnostic port for chart rendering resources. Concrete GDI+ object
	// types (Pen, Brush, Font, GraphicsPath, Region, StringFormat, ImageAttributes)
	// are replaced by these interfaces so alternate backends (SkiaSharp, future)
	// can be plugged in behind IChartRenderingEngine.
	//
	// Portable value types (Color, PointF, RectangleF, SizeF) and GDI+ enums are
	// intentionally NOT abstracted: they are cross-platform and do not trigger GDI+.

	/// <summary>
	/// Base for every backend-created rendering resource. Implementations may wrap a
	/// native, disposable object (e.g. a GDI+ <c>Pen</c> or a Skia <c>SKPaint</c>),
	/// so all resources are disposable.
	/// </summary>
	internal interface IRenderingResource : IDisposable
	{
	}
}
