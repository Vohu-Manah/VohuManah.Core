using ReportingServices.Rendering.ExcelRenderer;
using System;
using System.Drawing;
using System.IO;

namespace ReportingServices.Rendering.ImageRenderer
{
	/// <summary>
	/// Replaces System.Drawing.Image as ImageWriter's in-memory decoded-image
	/// representation, since Image.FromStream cannot run at all on non-Windows
	/// platforms (GDI+ cannot construct any System.Drawing object on Linux - see
	/// docs/platform-support.md's Phase 0 spike finding). On Windows this wraps a
	/// real GDI+ Image (unchanged behavior). On non-Windows it decodes via the
	/// IImageProvider abstraction already built for Excel's cross-platform image
	/// handling (docs/rendering-abstractions.md), giving Width/Height/resolution
	/// without needing GDI+ at all. Actually drawing this onto a page surface still
	/// requires a non-Windows raster surface, which does not exist yet - see
	/// tasks/image-renderer-cross-platform.md's Phase 2. This type only covers
	/// decode/metadata (Phase 1); IsGdiBacked is false on non-Windows and callers
	/// must skip the actual draw call until Phase 2 lands.
	/// </summary>
	internal sealed class PortableImage : IDisposable
	{
		private readonly Image m_gdiImage;

		private readonly byte[] m_bgra32Pixels;

		private int m_width;

		private int m_height;

		private float m_horizontalResolution;

		private float m_verticalResolution;

		internal Image GdiImage => m_gdiImage;

		internal bool IsGdiBacked => m_gdiImage != null;

		internal int Width => m_width;

		internal int Height => m_height;

		internal float HorizontalResolution => m_horizontalResolution;

		internal float VerticalResolution => m_verticalResolution;

		/// <summary>
		/// Tightly-packed top-down BGRA32 pixel buffer, decoded eagerly at construction time
		/// (non-Windows only) via IImageProvider.DecodeToBgra32 - Graphics.DrawImage's Skia
		/// overload consumes this directly. Eager rather than lazy so the object doesn't need
		/// to keep the source stream alive past FromStream returning; images are cached by
		/// ImageWriter (m_cachedImages) so this only runs once per distinct embedded image.
		/// </summary>
		internal byte[] Bgra32Pixels => m_bgra32Pixels;

		private PortableImage(Image gdiImage)
		{
			m_gdiImage = gdiImage;
			m_width = gdiImage.Width;
			m_height = gdiImage.Height;
			m_horizontalResolution = gdiImage.HorizontalResolution;
			m_verticalResolution = gdiImage.VerticalResolution;
		}

		private PortableImage(int width, int height, float horizontalResolution, float verticalResolution, byte[] bgra32Pixels)
		{
			m_width = width;
			m_height = height;
			m_horizontalResolution = horizontalResolution;
			m_verticalResolution = verticalResolution;
			m_bgra32Pixels = bgra32Pixels;
		}

		internal static PortableImage FromStream(Stream stream)
		{
			if (OperatingSystem.IsWindows())
			{
				return new PortableImage(Image.FromStream(stream));
			}
			// Buffered rather than passed straight through: IImageProvider.LoadImage
			// disposes its SKCodec internally, which also closes the stream it was
			// created from (SkiaSharp behavior) - a second call (DecodeToBgra32) on
			// the same stream instance would throw ObjectDisposedException.
			if (stream.CanSeek)
			{
				stream.Position = 0;
			}
			byte[] sourceBytes;
			using (MemoryStream buffer = new MemoryStream())
			{
				stream.CopyTo(buffer);
				sourceBytes = buffer.ToArray();
			}
			IImageProvider provider = ImageProviderFactory.CreateProvider();
			ImageMetadata metadata = provider.LoadImage(new MemoryStream(sourceBytes))
				?? throw new InvalidDataException("Unable to decode image stream.");
			byte[] pixels = provider.DecodeToBgra32(new MemoryStream(sourceBytes), metadata.Width, metadata.Height);
			return new PortableImage(metadata.Width, metadata.Height, metadata.HorizontalResolution, metadata.VerticalResolution, pixels);
		}

		internal static PortableImage FromGdiImage(Image gdiImage)
		{
			return new PortableImage(gdiImage);
		}

		/// <summary>
		/// Mirrors the GDI+ Bitmap.SetResolution call ImageWriter.SetResolution used to
		/// make directly - only meaningfully affects drawing on the (Windows-only, for
		/// now) GDI-backed path; on non-Windows this just updates the metadata this
		/// object reports, since there is no raster surface yet to draw onto.
		/// </summary>
		internal void SetResolution(float horizontalResolution, float verticalResolution)
		{
			if (m_gdiImage is Bitmap bitmap)
			{
				bitmap.SetResolution(horizontalResolution, verticalResolution);
			}
			m_horizontalResolution = horizontalResolution;
			m_verticalResolution = verticalResolution;
		}

		public void Dispose()
		{
			m_gdiImage?.Dispose();
		}
	}
}
