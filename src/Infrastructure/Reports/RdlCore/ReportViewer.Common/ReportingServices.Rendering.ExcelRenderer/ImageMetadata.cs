using ReportingServices.Rendering.ExcelRenderer.Excel;

namespace ReportingServices.Rendering.ExcelRenderer
{
	/// <summary>
	/// Image metadata extracted from image data.
	/// </summary>
	internal class ImageMetadata
	{
		/// <summary>
		/// Image width in pixels.
		/// </summary>
		public int Width { get; set; }

		/// <summary>
		/// Image height in pixels.
		/// </summary>
		public int Height { get; set; }

		/// <summary>
		/// Horizontal resolution in DPI.
		/// </summary>
		public float HorizontalResolution { get; set; }

		/// <summary>
		/// Vertical resolution in DPI.
		/// </summary>
		public float VerticalResolution { get; set; }

		/// <summary>
		/// Image format (BMP, GIF, JPEG, PNG, or Unknown).
		/// </summary>
		public ImageFormatType Format { get; set; }
	}
}
