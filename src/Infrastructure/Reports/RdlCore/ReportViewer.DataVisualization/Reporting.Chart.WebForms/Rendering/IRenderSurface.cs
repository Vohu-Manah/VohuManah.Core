using System;
using System.Drawing;
using System.IO;
using Reporting.Rendering;

namespace Reporting.Chart.WebForms.Rendering
{
	/// <summary>
	/// The raster draw target that replaces <c>new Bitmap(w, h)</c> + <c>Graphics.FromImage(...)</c>
	/// (ChartPicture.cs:733-734) and the final <c>image.Save(stream, format)</c> (Chart.cs:1313).
	/// On Skia this is an <c>SKSurface</c>; on GDI+ a <c>Bitmap</c>. Engine wiring lands in
	/// Milestone D1 — kept minimal here (A1 is interfaces only).
	/// </summary>
	internal interface IRenderSurface : IDisposable
	{
		int Width { get; }

		int Height { get; }

		float Dpi { get; }

		/// <summary>Encode the rendered surface to <paramref name="stream"/>.</summary>
		void Encode(Stream stream, ChartImageFormat format);

		/// <summary>
		/// Milestone F: the pre-paint background rectangle <c>ChartImage.GetImage</c>/<c>SaveImage</c>
		/// draw before <c>Paint</c> runs — a raw one-pixel-pen outline covering the whole surface
		/// (matches the pre-abstraction GDI+ behavior exactly, not a fill). Removes the last hard
		/// <c>GdiRenderSurface</c> downcast from those two methods so they work on either backend.
		/// </summary>
		void DrawBackgroundBorder(Color color);
	}

	/// <summary>
	/// Creates the platform draw surface. Selected per-platform by a factory
	/// (Milestone D2), mirroring the Excel <c>ImageProviderFactory</c> pattern.
	/// </summary>
	internal interface IRenderSurfaceFactory
	{
		IRenderSurface CreateRasterSurface(int width, int height, float dpi);

		/// <summary>
		/// EMF/EMF+ is a Windows-only format (no cross-platform equivalent). Callers must
		/// check this before requesting a metafile; non-Windows backends return <c>false</c>.
		/// </summary>
		bool SupportsMetafile { get; }
	}
}
