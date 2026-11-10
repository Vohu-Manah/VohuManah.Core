using System.IO;

namespace ReportingServices.Rendering.ExcelRenderer
{
	/// <summary>
	/// Platform-agnostic interface for image operations.
	/// Abstracts away platform-specific image libraries (System.Drawing vs alternatives).
	/// </summary>
	internal interface IImageProvider
	{
		/// <summary>
		/// Load an image from a stream and get its dimensions.
		/// </summary>
		ImageMetadata LoadImage(Stream imageStream);

		/// <summary>
		/// Get an image object suitable for the rendering backend.
		/// For chart rendering, this returns System.Drawing.Image (Windows).
		/// For future cross-platform charts, would return platform-appropriate type.
		/// </summary>
		object GetImageForChart(Stream imageStream);

		/// <summary>
		/// Decode an arbitrary image (BMP/GIF/TIFF/etc.) into a tightly-packed
		/// top-down pixel buffer (stride = width * 4, byte order B,G,R,A per pixel
		/// - the same in-memory layout as System.Drawing's PixelFormat.Format32bppArgb).
		/// Used by renderers (e.g. PDF) that need raw pixel access rather than a
		/// decoded Image/Bitmap object.
		/// </summary>
		byte[] DecodeToBgra32(Stream imageStream, int width, int height);

		/// <summary>
		/// Decode an arbitrary image and re-encode it as PNG. Used by renderers (e.g. Word)
		/// that only support a narrow embedded-image format set and need to transcode
		/// anything else (e.g. TIFF, BMP variants GDI+ would otherwise round-trip) to PNG.
		/// </summary>
		byte[] EncodeToPng(Stream imageStream);
	}
}
