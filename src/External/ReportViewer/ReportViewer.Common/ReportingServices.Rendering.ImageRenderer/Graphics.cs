using ReportingServices.OnDemandReportRendering;
using ReportingServices.Rendering.HPBProcessing;
using ReportingServices.Rendering.RPLProcessing;
using SkiaSharp;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace ReportingServices.Rendering.ImageRenderer
{
	/// <summary>
	/// Phases 2-3 of the IMAGE renderer's Skia-backend migration (tasks/image-renderer-cross-platform.md):
	/// every method here branches on OperatingSystem.IsWindows() - the Windows branch is the
	/// original GDI+ code, byte-for-byte unchanged, so Windows behavior/output is identical to
	/// before this migration. The non-Windows branch draws onto an SKBitmap/SKCanvas instead,
	/// covering BMP/GIF/JPEG/PNG only (narrowed scope decision, 2026-07-28) - TIFF has no
	/// SkiaSharp encoder and stays Windows-only, same as EMF. DrawText (Phase 3) is the
	/// non-Windows text path, reusing PDFWriter's existing ShapedFontCache/SkiaCachedFont
	/// infrastructure for wrapping/measurement - see ImageWriter.DrawWrappedText/DrawWrappedRichText.
	/// </summary>
	internal class Graphics : GraphicsBase
	{
		private EncoderParameters m_encoderParameters;

		private Bitmap m_firstImage;

		private Bitmap m_pageBitmap;

		private static ImageCodecInfo[] m_encoders;

		private SKBitmap m_skBitmap;

		private SKCanvas m_skCanvas;

		private int m_skBaseSaveCount;

		internal Graphics(float dpiX, float dpiY)
			: base(dpiX, dpiY)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (m_firstImage != null)
				{
					m_firstImage.Dispose();
					m_firstImage = null;
				}
				if (m_encoderParameters != null)
				{
					m_encoderParameters.Dispose();
					m_encoderParameters = null;
				}
				if (m_pageBitmap != null)
				{
					m_pageBitmap.Dispose();
					m_pageBitmap = null;
				}
				DisposeSkiaPage();
			}
			base.Dispose(disposing);
		}

		internal virtual void Save(Stream outputStream, PaginationSettings.FormatEncoding outputFormat)
		{
			if (OperatingSystem.IsWindows())
			{
				SaveGdi(outputStream, outputFormat);
				return;
			}
			SKBitmap bitmap = m_skBitmap;
			m_skCanvas?.Dispose();
			m_skCanvas = null;
			m_skBitmap = null;
			try
			{
				// SkiaSharp's SKImage.Encode only actually supports Jpeg/Png (empirically
				// confirmed under WSL - Bmp/Gif both return null data, silently). BMP/GIF
				// output stays Windows-only for now alongside TIFF/EMF - see
				// tasks/image-renderer-cross-platform.md.
				SKEncodedImageFormat format = outputFormat switch
				{
					PaginationSettings.FormatEncoding.JPEG => SKEncodedImageFormat.Jpeg,
					PaginationSettings.FormatEncoding.PNG => SKEncodedImageFormat.Png,
					_ => throw new NotSupportedException($"IMAGE output format '{outputFormat}' is not yet supported on non-Windows platforms - see tasks/image-renderer-cross-platform.md."),
				};
				using SKImage image = SKImage.FromBitmap(bitmap);
				using SKData data = image.Encode(format, 100)
					?? throw new InvalidOperationException($"SkiaSharp failed to encode the page as {format}.");
				data.SaveTo(outputStream);
				outputStream.Flush();
			}
			finally
			{
				bitmap?.Dispose();
			}
		}

		private void SaveGdi(Stream outputStream, PaginationSettings.FormatEncoding outputFormat)
		{
			Bitmap bitmap = m_pageBitmap;
			bool flag = true;
			try
			{
				switch (outputFormat)
				{
				case PaginationSettings.FormatEncoding.BMP:
					bitmap.Save(outputStream, ImageFormat.Bmp);
					break;
				case PaginationSettings.FormatEncoding.GIF:
					bitmap.Save(outputStream, ImageFormat.Gif);
					break;
				case PaginationSettings.FormatEncoding.JPEG:
					bitmap.Save(outputStream, ImageFormat.Jpeg);
					break;
				case PaginationSettings.FormatEncoding.PNG:
					bitmap.Save(outputStream, ImageFormat.Png);
					break;
				case PaginationSettings.FormatEncoding.TIFF:
					if (m_firstImage == null)
					{
						m_firstImage = bitmap;
						flag = false;
						m_pageBitmap = null;
						m_encoderParameters = new EncoderParameters(2);
						m_encoderParameters.Param[0] = new EncoderParameter(Encoder.SaveFlag, 18L);
						m_encoderParameters.Param[1] = new EncoderParameter(Encoder.ColorDepth, 24L);
						m_firstImage.Save(outputStream, GetEncoderInfo("image/tiff"), m_encoderParameters);
						EncoderParameter encoderParameter = m_encoderParameters.Param[0];
						m_encoderParameters.Param[0] = new EncoderParameter(Encoder.SaveFlag, 23L);
						if (encoderParameter != null)
						{
							encoderParameter.Dispose();
							encoderParameter = null;
						}
					}
					else
					{
						m_firstImage.SaveAdd(bitmap, m_encoderParameters);
					}
					break;
				}
				outputStream.Flush();
			}
			finally
			{
				if (flag && bitmap != null)
				{
					bitmap.Dispose();
					bitmap = null;
					m_pageBitmap = null;
				}
			}
		}

		internal void NewPage(float pageWidth, float pageHeight, int dpiX, int dpiY)
		{
			if (OperatingSystem.IsWindows())
			{
				NewPageGdi(pageWidth, pageHeight);
				return;
			}
			DisposeSkiaPage();
			SKImageInfo info = new SKImageInfo(ConvertToPixels(pageWidth), ConvertToPixels(pageHeight), SKColorType.Bgra8888, SKAlphaType.Opaque);
			m_skBitmap = new SKBitmap(info);
			m_skCanvas = new SKCanvas(m_skBitmap);
			m_skCanvas.Clear(SKColors.White);
			m_skBaseSaveCount = m_skCanvas.Save();
		}

		private void NewPageGdi(float pageWidth, float pageHeight)
		{
			if (m_graphicsBase != null)
			{
				ReleaseCachedHdc(releaseHdc: true);
				m_graphicsBase.Dispose();
				m_graphicsBase = null;
			}
			if (m_pageBitmap != null)
			{
				m_pageBitmap.Dispose();
				m_pageBitmap = null;
			}
			// Builds the per-page raster surface as a plain System.Drawing.Bitmap rather than
			// via raw Win32 GetDC/CreateCompatibleDC/CreateDIBSection HBITMAP interop (as this
			// used to). That HBITMAP path is unavailable on non-Windows System.Drawing.Common
			// (libgdiplus has no HDC/HBITMAP concept) and failed immediately, before any drawing,
			// on every page for every raster format (TIFF included) - see
			// tasks/image-renderer-cross-platform.md. Bitmap+Graphics.FromImage is the same
			// portable construction GraphicsBase.EnsureGraphics already uses for its scratch
			// HDC, and is what Chart's GdiRenderSurface/SkiaRenderSurface pair already models.
			// Format32bppRgb (no per-pixel alpha), not the Bitmap(w, h) default of
			// Format32bppArgb - the old CreateDIBSection-backed HDC surface had no real alpha
			// channel, and drawing alpha-edged content (e.g. an embedded chart PNG) onto a
			// true-alpha surface instead changes edge-pixel compositing enough to fail
			// SunburstChartRdlTests's pixel-diff baseline.
			m_pageBitmap = new Bitmap(ConvertToPixels(pageWidth), ConvertToPixels(pageHeight), PixelFormat.Format32bppRgb);
			m_pageBitmap.SetResolution(base.DpiX, base.DpiY);
			m_graphicsBase = System.Drawing.Graphics.FromImage(m_pageBitmap);
			SetGraphicsProperties(m_graphicsBase);
			m_graphicsBase.Clear(Color.White);
		}

		private void DisposeSkiaPage()
		{
			m_skCanvas?.Dispose();
			m_skCanvas = null;
			m_skBitmap?.Dispose();
			m_skBitmap = null;
		}

		internal void DrawLine(Pen pen, float x1, float y1, float x2, float y2)
		{
			ReleaseCachedHdc(releaseHdc: true);
			ExecuteSync(delegate
			{
				m_graphicsBase.DrawLine(pen, ConvertToPixels(x1), ConvertToPixels(y1), ConvertToPixels(x2), ConvertToPixels(y2));
			});
		}

		/// <summary>Non-Windows sibling of <see cref="DrawLine(Pen, float, float, float, float)"/> - takes primitives instead of a pre-built Pen, since Pen construction itself needs GDI+.</summary>
		internal void DrawLine(Color color, float sizeInPixels, RPLFormat.BorderStyles style, float x1, float y1, float x2, float y2)
		{
			ExecuteSync(delegate
			{
				using SKPaint paint = CreateSkiaStrokePaint(color, sizeInPixels, style);
				m_skCanvas.DrawLine(ConvertToPixels(x1), ConvertToPixels(y1), ConvertToPixels(x2), ConvertToPixels(y2), paint);
			});
		}

		internal void DrawImage(System.Drawing.Image image, RectangleF destination, RectangleF source)
		{
			DrawImage(image, destination, source, tile: true);
		}

		internal void DrawImage(System.Drawing.Image image, RectangleF destination, RectangleF source, bool tile)
		{
			ReleaseCachedHdc(releaseHdc: true);
			ExecuteSync(delegate
			{
				ImageAttributes imageAttributes = null;
				try
				{
					if (tile)
					{
						imageAttributes = new ImageAttributes();
						imageAttributes.SetWrapMode(WrapMode.Tile);
					}
					PointF[] destPoints = new PointF[3]
					{
						new PointF(ConvertToPixels(destination.Location.X), ConvertToPixels(destination.Location.Y)),
						new PointF(ConvertToPixels(destination.Location.X + destination.Width), ConvertToPixels(destination.Location.Y)),
						new PointF(ConvertToPixels(destination.Location.X), ConvertToPixels(destination.Location.Y + destination.Height))
					};
					m_graphicsBase.DrawImage(image, destPoints, source, GraphicsUnit.Pixel, imageAttributes);
				}
				finally
				{
					if (imageAttributes != null)
					{
						imageAttributes.Dispose();
						imageAttributes = null;
					}
				}
			});
		}

		/// <summary>
		/// Non-Windows sibling of <see cref="DrawImage(System.Drawing.Image, RectangleF, RectangleF, bool)"/> -
		/// takes an already-decoded BGRA32 pixel buffer (PortableImage.GetBgra32Pixels) instead of a
		/// System.Drawing.Image, since Image construction itself needs GDI+. Does not implement true
		/// tiling (GDI+'s ImageAttributes.SetWrapMode(WrapMode.Tile)) - a documented, honest gap for
		/// the rarely-exercised background-repeat case, same "approximate but disclosed" precedent as
		/// SkiaHatchBrush/SkiaPathGradientBrush (docs/decisions.md).
		/// </summary>
		internal void DrawImage(byte[] bgra32Pixels, int sourceWidth, int sourceHeight, RectangleF destination, RectangleF source)
		{
			ExecuteSync(delegate
			{
				SKImageInfo info = new SKImageInfo(sourceWidth, sourceHeight, SKColorType.Bgra8888, SKAlphaType.Unpremul);
				using SKBitmap sourceBitmap = new SKBitmap(info);
				GCHandle handle = GCHandle.Alloc(bgra32Pixels, GCHandleType.Pinned);
				try
				{
					sourceBitmap.InstallPixels(info, handle.AddrOfPinnedObject(), info.RowBytes);
					SKRect destRect = SKRect.Create(ConvertToPixels(destination.X), ConvertToPixels(destination.Y), ConvertToPixels(destination.Width), ConvertToPixels(destination.Height));
					SKRect srcRect = SKRect.Create(source.X, source.Y, source.Width, source.Height);
					m_skCanvas.DrawBitmap(sourceBitmap, srcRect, destRect);
				}
				finally
				{
					handle.Free();
				}
			});
		}

		internal void DrawRectangle(Pen pen, RectangleF rectangle)
		{
			ReleaseCachedHdc(releaseHdc: true);
			ExecuteSync(delegate
			{
				m_graphicsBase.DrawRectangle(pen, ConvertToPixels(rectangle.X), ConvertToPixels(rectangle.Y), ConvertToPixels(rectangle.Width), ConvertToPixels(rectangle.Height));
			});
		}

		/// <summary>Non-Windows sibling of <see cref="DrawRectangle(Pen, RectangleF)"/> - see <see cref="DrawLine(Color, float, RPLFormat.BorderStyles, float, float, float, float)"/>.</summary>
		internal void DrawRectangle(Color color, float sizeInPixels, RPLFormat.BorderStyles style, RectangleF rectangle)
		{
			ExecuteSync(delegate
			{
				using SKPaint paint = CreateSkiaStrokePaint(color, sizeInPixels, style);
				m_skCanvas.DrawRect(SKRect.Create(ConvertToPixels(rectangle.X), ConvertToPixels(rectangle.Y), ConvertToPixels(rectangle.Width), ConvertToPixels(rectangle.Height)), paint);
			});
		}

		internal void FillPolygon(Brush brush, PointF[] polygon)
		{
			ReleaseCachedHdc(releaseHdc: true);
			ExecuteSync(delegate
			{
				Point[] array = new Point[polygon.Length];
				for (int i = 0; i < polygon.Length; i++)
				{
					PointF pointF = polygon[i];
					array[i].X = ConvertToPixels(pointF.X);
					array[i].Y = ConvertToPixels(pointF.Y);
				}
				m_graphicsBase.FillPolygon(brush, array);
			});
		}

		/// <summary>Non-Windows sibling of <see cref="FillPolygon(Brush, PointF[])"/> - takes a Color instead of a pre-built Brush.</summary>
		internal void FillPolygon(Color color, PointF[] polygon)
		{
			ExecuteSync(delegate
			{
				using SKPaint paint = new SKPaint { Style = SKPaintStyle.Fill, Color = ToSKColor(color) };
				using SKPath path = new SKPath { FillType = SKPathFillType.EvenOdd };
				path.MoveTo(ConvertToPixels(polygon[0].X), ConvertToPixels(polygon[0].Y));
				for (int i = 1; i < polygon.Length; i++)
				{
					path.LineTo(ConvertToPixels(polygon[i].X), ConvertToPixels(polygon[i].Y));
				}
				path.Close();
				m_skCanvas.DrawPath(path, paint);
			});
		}

		internal void FillRectangle(Brush brush, RectangleF rectangle)
		{
			ReleaseCachedHdc(releaseHdc: true);
			ExecuteSync(delegate
			{
				m_graphicsBase.FillRectangle(brush, ConvertToPixels(rectangle.X), ConvertToPixels(rectangle.Y), ConvertToPixels(rectangle.Width), ConvertToPixels(rectangle.Height));
			});
		}

		/// <summary>Non-Windows sibling of <see cref="FillRectangle(Brush, RectangleF)"/> - takes a Color instead of a pre-built Brush.</summary>
		internal void FillRectangle(Color color, RectangleF rectangle)
		{
			ExecuteSync(delegate
			{
				using SKPaint paint = new SKPaint { Style = SKPaintStyle.Fill, Color = ToSKColor(color) };
				m_skCanvas.DrawRect(SKRect.Create(ConvertToPixels(rectangle.X), ConvertToPixels(rectangle.Y), ConvertToPixels(rectangle.Width), ConvertToPixels(rectangle.Height)), paint);
			});
		}

		internal void ResetClipAndTransform(RectangleF bounds)
		{
			ReleaseCachedHdc(releaseHdc: true);
			ExecuteSync(delegate
			{
				if (OperatingSystem.IsWindows())
				{
					m_graphicsBase.ResetClip();
					m_graphicsBase.ResetTransform();
					System.Drawing.Rectangle clip = new System.Drawing.Rectangle(ConvertToPixels(bounds.X), ConvertToPixels(bounds.Y), ConvertToPixels(bounds.Width), ConvertToPixels(bounds.Height));
					m_graphicsBase.SetClip(clip);
					using (Matrix matrix = new Matrix())
					{
						matrix.Translate(clip.Left, clip.Top);
						m_graphicsBase.Transform = matrix;
					}
					return;
				}
				m_skCanvas.RestoreToCount(m_skBaseSaveCount);
				m_skCanvas.Save();
				SKRect clipRect = SKRect.Create(ConvertToPixels(bounds.X), ConvertToPixels(bounds.Y), ConvertToPixels(bounds.Width), ConvertToPixels(bounds.Height));
				m_skCanvas.ClipRect(clipRect);
				m_skCanvas.Translate(clipRect.Left, clipRect.Top);
			});
		}

		internal void RotateTransform(float angle)
		{
			ReleaseCachedHdc(releaseHdc: true);
			ExecuteSync(delegate
			{
				if (OperatingSystem.IsWindows())
				{
					m_graphicsBase.RotateTransform(angle);
					return;
				}
				m_skCanvas.RotateDegrees(angle);
			});
		}

		/// <summary>
		/// Non-Windows-only text drawing (Phase 3) - draws a single line at the given baseline
		/// position using an already-resolved SkiaCachedFont (see ImageWriter.DrawWrappedText/
		/// DrawWrappedRichText, which own wrapping/measurement via ShapedFontCache). No Windows
		/// equivalent here: on Windows, ImageWriter.DrawTextRun keeps using the original Win32
		/// HDC/Uniscribe LineBreaker/TextBox pipeline, unaffected by this method's existence.
		/// </summary>
		internal void DrawText(string text, ReportingServices.Rendering.RichText.SkiaCachedFont font, Color color, float xPixels, float baselineYPixels)
		{
			ExecuteSync(delegate
			{
				using SKPaint paint = new SKPaint { Color = ToSKColor(color), IsAntialias = true };
				m_skCanvas.DrawText(text, xPixels, baselineYPixels, font.Font, paint);
			});
		}

		internal void EndReport(PaginationSettings.FormatEncoding outputFormat)
		{
			if (outputFormat == PaginationSettings.FormatEncoding.TIFF)
			{
				EncoderParameter encoderParameter = m_encoderParameters.Param[0];
				m_encoderParameters.Param[0] = new EncoderParameter(Encoder.SaveFlag, 20L);
				if (encoderParameter != null)
				{
					encoderParameter.Dispose();
					encoderParameter = null;
				}
				m_firstImage.SaveAdd(m_encoderParameters);
			}
		}

		protected static void SetGraphicsProperties(System.Drawing.Graphics graphics)
		{
			graphics.CompositingMode = CompositingMode.SourceOver;
			graphics.PageUnit = GraphicsUnit.Pixel;
			graphics.PixelOffsetMode = PixelOffsetMode.Default;
			graphics.SmoothingMode = SmoothingMode.Default;
			graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
		}

		private static ImageCodecInfo GetEncoderInfo(string mimeType)
		{
			ImageCodecInfo[] encoders = GetGdiImageEncoders();
			if (encoders == null)
			{
				return null;
			}
			for (int i = 0; i < encoders.Length; i++)
			{
				if (encoders[i].MimeType == mimeType)
				{
					return encoders[i];
				}
			}
			return null;
		}

		// Lazily initialized (rather than a static field initializer) so merely loading this
		// type doesn't call into GDI+ - only Save's Windows-only TIFF branch ever calls this.
		private static ImageCodecInfo[] GetGdiImageEncoders()
		{
			return m_encoders ??= ImageCodecInfo.GetImageEncoders();
		}

		/// <summary>Mirrors GdiPen/SkiaPen's DashStyle->SKPathEffect conversion (Rendering/Skia/SkiaPen.cs) for visual consistency with Chart's established dash-pattern ratios.</summary>
		private static SKPaint CreateSkiaStrokePaint(Color color, float widthPixels, RPLFormat.BorderStyles style)
		{
			SKPaint paint = new SKPaint
			{
				Style = SKPaintStyle.Stroke,
				Color = ToSKColor(color),
				StrokeWidth = widthPixels,
			};
			float unit = widthPixels <= 0f ? 1f : widthPixels;
			switch (style)
			{
			case RPLFormat.BorderStyles.Dashed:
				paint.PathEffect = SKPathEffect.CreateDash(new[] { 3f * unit, 1f * unit }, 0f);
				break;
			case RPLFormat.BorderStyles.Dotted:
				paint.PathEffect = SKPathEffect.CreateDash(new[] { 1f * unit, 1f * unit }, 0f);
				break;
			}
			return paint;
		}

		private static SKColor ToSKColor(Color color)
		{
			return new SKColor(color.R, color.G, color.B, color.A);
		}
	}
}
