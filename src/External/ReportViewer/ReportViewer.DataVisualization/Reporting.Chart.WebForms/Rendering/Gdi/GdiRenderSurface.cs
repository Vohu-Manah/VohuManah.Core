using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Reporting.Rendering;

namespace Reporting.Chart.WebForms.Rendering.Gdi
{
	/// <summary>
	/// Milestone A2 adapter — replaces <c>new Bitmap(w, h)</c> + <c>Graphics.FromImage(...)</c>
	/// (ChartPicture.cs:733-734) behind <see cref="IRenderSurface"/>.
	/// </summary>
	internal sealed class GdiRenderSurface : IRenderSurface
	{
		internal Bitmap NativeBitmap { get; }

		internal Graphics NativeGraphics { get; }

		internal GdiRenderSurface(int width, int height, float dpi)
		{
			NativeBitmap = new Bitmap(width, height);
			NativeBitmap.SetResolution(dpi, dpi);
			NativeGraphics = Graphics.FromImage(NativeBitmap);
		}

		public int Width => NativeBitmap.Width;

		public int Height => NativeBitmap.Height;

		public float Dpi => NativeBitmap.HorizontalResolution;

		public void Encode(Stream stream, ChartImageFormat format) => NativeBitmap.Save(stream, ToImageFormat(format));

		public void DrawBackgroundBorder(Color color)
		{
			using Pen pen = new Pen(color);
			NativeGraphics.DrawRectangle(pen, 0, 0, Width, Height);
		}

		// Emf/EmfPlus/EmfDual are metafile formats — they require a Metafile-backed
		// surface, not this raster (Bitmap) surface. Callers must check
		// IRenderSurfaceFactory.SupportsMetafile and use the metafile path (not yet
		// implemented — see chart-gdi-type-abstraction.md Milestone D) for those formats.
		private static ImageFormat ToImageFormat(ChartImageFormat format) => format switch
		{
			ChartImageFormat.Jpeg => ImageFormat.Jpeg,
			ChartImageFormat.Bmp => ImageFormat.Bmp,
			_ => ImageFormat.Png,
		};

		public void Dispose()
		{
			NativeGraphics.Dispose();
			NativeBitmap.Dispose();
		}
	}

	/// <summary>Milestone A2 adapter — creates <see cref="GdiRenderSurface"/> instances. EMF/EMF+ remains Windows-only.</summary>
	internal sealed class GdiRenderSurfaceFactory : IRenderSurfaceFactory
	{
		public IRenderSurface CreateRasterSurface(int width, int height, float dpi) => new GdiRenderSurface(width, height, dpi);

		public bool SupportsMetafile => true;
	}
}
