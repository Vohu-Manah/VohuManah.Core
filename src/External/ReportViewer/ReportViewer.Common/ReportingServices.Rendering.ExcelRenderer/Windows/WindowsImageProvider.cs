using ReportingServices.Rendering.ExcelRenderer.Excel;
using System;
using System.Drawing;
using System.IO;

namespace ReportingServices.Rendering.ExcelRenderer.Windows
{
	/// <summary>
	/// Windows-specific image provider using System.Drawing.
	/// Used for chart rendering which depends on System.Drawing.Image.
	/// </summary>
	[System.Runtime.Versioning.SupportedOSPlatform("windows")]
	internal class WindowsImageProvider : IImageProvider
	{
		/// <summary>
		/// Load an image from a stream and get its dimensions.
		/// </summary>
		public ImageMetadata LoadImage(Stream imageStream)
		{
			if (imageStream == null || imageStream.Length == 0)
				return null;

			imageStream.Position = 0;
			Image gdiImage = null;
			try
			{
				gdiImage = Image.FromStream(imageStream);

				var metadata = new ImageMetadata
				{
					Width = gdiImage.Width,
					Height = gdiImage.Height,
					HorizontalResolution = gdiImage.HorizontalResolution,
					VerticalResolution = gdiImage.VerticalResolution,
					Format = DetermineFormat(gdiImage.RawFormat)
				};

				return metadata;
			}
			finally
			{
				gdiImage?.Dispose();
			}
		}

		/// <summary>
		/// Get an image object suitable for the rendering backend.
		/// For chart rendering, this returns System.Drawing.Image.
		/// </summary>
		public object GetImageForChart(Stream imageStream)
		{
			if (imageStream == null || imageStream.Length == 0)
				return null;

			imageStream.Position = 0;
			return Image.FromStream(imageStream);
		}

		/// <summary>
		/// Decode into a tightly-packed top-down BGRA32 buffer via System.Drawing/GDI+.
		/// </summary>
		public byte[] DecodeToBgra32(Stream imageStream, int width, int height)
		{
			byte[] buffer = new byte[width * height * 4];
			using (Bitmap bitmap = new Bitmap(Image.FromStream(imageStream)))
			{
				System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, width, height);
				System.Drawing.Imaging.BitmapData bitmapData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
				try
				{
					int rowBytes = width * 4;
					for (int row = 0; row < height; row++)
					{
						IntPtr rowStart = IntPtr.Add(bitmapData.Scan0, row * bitmapData.Stride);
						System.Runtime.InteropServices.Marshal.Copy(rowStart, buffer, row * rowBytes, rowBytes);
					}
				}
				finally
				{
					bitmap.UnlockBits(bitmapData);
				}
			}
			return buffer;
		}

		/// <summary>
		/// Decode an arbitrary image and re-encode it as PNG via System.Drawing/GDI+.
		/// </summary>
		public byte[] EncodeToPng(Stream imageStream)
		{
			imageStream.Position = 0;
			using (Image gdiImage = Image.FromStream(imageStream))
			using (MemoryStream memoryStream = new MemoryStream())
			{
				gdiImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
				return memoryStream.ToArray();
			}
		}

		private static ImageFormatType DetermineFormat(System.Drawing.Imaging.ImageFormat rawFormat)
		{
			if (rawFormat == null)
				return ImageFormatType.Unknown;

			if (rawFormat.Guid == System.Drawing.Imaging.ImageFormat.Bmp.Guid)
				return ImageFormatType.Bmp;
			if (rawFormat.Guid == System.Drawing.Imaging.ImageFormat.Gif.Guid)
				return ImageFormatType.Gif;
			if (rawFormat.Guid == System.Drawing.Imaging.ImageFormat.Jpeg.Guid)
				return ImageFormatType.Jpeg;
			if (rawFormat.Guid == System.Drawing.Imaging.ImageFormat.Png.Guid)
				return ImageFormatType.Png;

			return ImageFormatType.Unknown;
		}
	}
}
