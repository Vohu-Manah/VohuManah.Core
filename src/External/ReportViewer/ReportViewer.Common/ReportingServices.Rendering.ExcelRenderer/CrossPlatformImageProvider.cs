using ReportingServices.Rendering.ExcelRenderer.Excel;
using SkiaSharp;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ReportingServices.Rendering.ExcelRenderer
{
	/// <summary>
	/// Cross-platform image provider using SkiaSharp.
	/// Used for non-chart image operations (Excel embedded images, web rendering).
	/// Returns null for chart operations (not supported on non-Windows platforms).
	/// </summary>
	internal class CrossPlatformImageProvider : IImageProvider
	{
		/// <summary>
		/// Load an image from a stream and get its dimensions.
		/// </summary>
		public ImageMetadata LoadImage(Stream imageStream)
		{
			if (imageStream == null || imageStream.Length == 0)
				return null;

			try
			{
				imageStream.Position = 0;
				using SKCodec codec = SKCodec.Create(imageStream);

				if (codec == null)
					return null;

				imageStream.Position = 0;

				// SkiaSharp carries no embedded resolution metadata (see docs/decisions.md,
				// "ImageLoader's DPI-mismatch rescaling was dropped, not ported"); assume the
				// same fixed 96 DPI baseline used elsewhere in the cross-platform renderers.
				var metadata = new ImageMetadata
				{
					Width = codec.Info.Width,
					Height = codec.Info.Height,
					HorizontalResolution = 96f,
					VerticalResolution = 96f,
					Format = DetermineFormat(codec.EncodedFormat)
				};

				return metadata;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Chart rendering is Windows-only in current architecture.
		/// Return null on non-Windows platforms.
		/// Future: Alternative chart libraries may provide cross-platform support.
		/// </summary>
		public object GetImageForChart(Stream imageStream)
		{
			return null;
		}

		/// <summary>
		/// Decode into a tightly-packed top-down BGRA32 buffer via SkiaSharp.
		/// SKColorType.Bgra8888 with SKAlphaType.Unpremul has the same in-memory
		/// byte order (B,G,R,A) as System.Drawing's Format32bppArgb, so the pixel
		/// buffer can be copied out directly with no channel reordering.
		/// </summary>
		public byte[] DecodeToBgra32(Stream imageStream, int width, int height)
		{
			imageStream.Position = 0;
			var info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
			using (SKCodec codec = SKCodec.Create(imageStream))
			using (SKBitmap bitmap = new SKBitmap(info))
			{
				codec.GetPixels(info, bitmap.GetPixels());

				byte[] buffer = new byte[width * height * 4];
				int rowBytes = width * 4;
				IntPtr pixels = bitmap.GetPixels();
				for (int row = 0; row < height; row++)
				{
					IntPtr rowStart = IntPtr.Add(pixels, row * bitmap.RowBytes);
					Marshal.Copy(rowStart, buffer, row * rowBytes, rowBytes);
				}
				return buffer;
			}
		}

		/// <summary>
		/// Decode an arbitrary image and re-encode it as PNG via SkiaSharp.
		/// </summary>
		public byte[] EncodeToPng(Stream imageStream)
		{
			imageStream.Position = 0;
			using (SKBitmap bitmap = SKBitmap.Decode(imageStream))
			using (SKData data = bitmap.Encode(SKEncodedImageFormat.Png, 100))
			{
				return data.ToArray();
			}
		}

		private static ImageFormatType DetermineFormat(SKEncodedImageFormat format)
		{
			return format switch
			{
				SKEncodedImageFormat.Bmp => ImageFormatType.Bmp,
				SKEncodedImageFormat.Gif => ImageFormatType.Gif,
				SKEncodedImageFormat.Jpeg => ImageFormatType.Jpeg,
				SKEncodedImageFormat.Png => ImageFormatType.Png,
				_ => ImageFormatType.Unknown
			};
		}
	}
}
