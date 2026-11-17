using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using SkiaSharp;
using Reporting.Rendering;

namespace Reporting.Chart.WebForms.Rendering.Skia
{
	/// <summary>
	/// Spike (tasks/chart-cross-platform-implementation.md Phase 0) — the SkiaSharp
	/// implementation of <see cref="IDrawingResourceFactory"/>, sibling to
	/// <c>GdiResourceFactory</c>. Every resource it creates wraps a SkiaSharp object
	/// instead of a <see cref="System.Drawing"/> one, so a caller that only depends on
	/// this factory + the <c>Rendering.*</c> interfaces never triggers GDI+
	/// initialization — proving that path is constructible on Linux (see spike report;
	/// GDI+ itself cannot be constructed there even with libgdiplus installed).
	/// </summary>
	internal sealed class SkiaResourceFactory : IDrawingResourceFactory
	{
		public IPen CreatePen(Color color, float width) => new SkiaPen(color, width);

		/// <summary>Real (E1) for every brush kind this backend can produce a native <see cref="SKPaint"/> for.</summary>
		public IPen CreatePen(IBrush brush, float width)
		{
			SKPaint source = brush switch
			{
				SkiaSolidBrush b => b.NativePaint,
				SkiaTextureBrush b => b.NativePaint,
				SkiaLinearGradientBrush b => b.NativePaint,
				SkiaPathGradientBrush b => b.NativePaint,
				SkiaHatchBrush b => b.NativePaint,
				_ => throw new NotSupportedException($"CreatePen(IBrush, float): unsupported brush kind {brush.GetType().Name}."),
			};
			return new SkiaPen(source, width);
		}

		public ISolidBrush CreateSolidBrush(Color color) => new SkiaSolidBrush(color);

		/// <summary>Real (Milestone E1, 2026-07-23) — see <see cref="SkiaLinearGradientBrush"/>.</summary>
		public ILinearGradientBrush CreateLinearGradientBrush(RectangleF rect, Color startColor, Color endColor, float angle) =>
			new SkiaLinearGradientBrush(rect, startColor, endColor, angle);

		/// <summary>Real (Milestone E1, 2026-07-23) — see <see cref="SkiaLinearGradientBrush"/>.</summary>
		public ILinearGradientBrush CreateLinearGradientBrush(PointF point1, PointF point2, Color color1, Color color2) =>
			new SkiaLinearGradientBrush(point1, point2, color1, color2);

		/// <summary>Real (E1) — a full-image tiled bitmap shader; GDI+'s <c>TextureBrush(Image, WrapMode)</c> equivalent.</summary>
		public ITextureBrush CreateTextureBrush(IChartImage image, WrapMode wrapMode)
		{
			SKBitmap bitmap = ((SkiaChartImage)image).NativeBitmap;
			(SKShaderTileMode tmx, SKShaderTileMode tmy) = ToSkiaTileModes(wrapMode);
			SKPaint paint = new SKPaint
			{
				Style = SKPaintStyle.Fill,
				IsAntialias = true,
				Shader = SKShader.CreateBitmap(bitmap, tmx, tmy),
			};
			return new SkiaTextureBrush(paint, wrapMode);
		}

		/// <summary>
		/// Real (E1) — GDI+'s <c>TextureBrush(Image, RectangleF, ImageAttributes)</c> equivalent: the
		/// bitmap is mapped onto <paramref name="rect"/> via a scale+translate local matrix, and an
		/// optional colour-key (<see cref="SkiaImageDrawOptions.TransparentColor"/>) is applied as an
		/// exact per-pixel RGB match against a bitmap copy (no tolerance/anti-alias fringe, unlike GDI+'s
		/// <c>ColorAdjustType</c> pipeline — an approximation, not a byte-for-byte port).
		/// </summary>
		public ITextureBrush CreateTextureBrush(IChartImage image, RectangleF rect, IImageDrawOptions options)
		{
			SKBitmap bitmap = ((SkiaChartImage)image).NativeBitmap;
			SkiaImageDrawOptions skiaOptions = options as SkiaImageDrawOptions;
			SKBitmap ownedBitmap = null;
			if (skiaOptions?.TransparentColor is Color transparentColor)
			{
				bitmap = ownedBitmap = ApplyColorKey(bitmap, SkiaConvert.ToSKColor(transparentColor));
			}
			(SKShaderTileMode tmx, SKShaderTileMode tmy) = ToSkiaTileModes(skiaOptions?.WrapMode ?? WrapMode.Clamp);
			float scaleX = bitmap.Width == 0 ? 1f : rect.Width / bitmap.Width;
			float scaleY = bitmap.Height == 0 ? 1f : rect.Height / bitmap.Height;
			SKMatrix localMatrix = SKMatrix.CreateScaleTranslation(scaleX, scaleY, rect.X, rect.Y);
			SKPaint paint = new SKPaint
			{
				Style = SKPaintStyle.Fill,
				IsAntialias = true,
				Shader = SKShader.CreateBitmap(bitmap, tmx, tmy, localMatrix),
			};
			return new SkiaTextureBrush(paint, skiaOptions?.WrapMode ?? WrapMode.Clamp, ownedBitmap);
		}

		private static SKBitmap ApplyColorKey(SKBitmap source, SKColor keyColor)
		{
			SKBitmap result = source.Copy();
			for (int y = 0; y < result.Height; y++)
			{
				for (int x = 0; x < result.Width; x++)
				{
					SKColor pixel = result.GetPixel(x, y);
					if (pixel.Red == keyColor.Red && pixel.Green == keyColor.Green && pixel.Blue == keyColor.Blue)
					{
						result.SetPixel(x, y, SKColors.Transparent);
					}
				}
			}
			return result;
		}

		/// <summary>GDI+'s <c>TileFlipX/Y</c> flip alternating tiles — Skia's <see cref="SKShaderTileMode.Mirror"/> is the direct per-axis equivalent.</summary>
		private static (SKShaderTileMode x, SKShaderTileMode y) ToSkiaTileModes(WrapMode mode) => mode switch
		{
			WrapMode.Tile => (SKShaderTileMode.Repeat, SKShaderTileMode.Repeat),
			WrapMode.TileFlipX => (SKShaderTileMode.Mirror, SKShaderTileMode.Repeat),
			WrapMode.TileFlipY => (SKShaderTileMode.Repeat, SKShaderTileMode.Mirror),
			WrapMode.TileFlipXY => (SKShaderTileMode.Mirror, SKShaderTileMode.Mirror),
			_ => (SKShaderTileMode.Clamp, SKShaderTileMode.Clamp),
		};

		/// <summary>Real (Milestone E1, 2026-07-23) — see <see cref="SkiaHatchBrush"/>/<see cref="SkiaHatchPatterns"/>.</summary>
		public IHatchBrush CreateHatchBrush(HatchStyle style, Color foreColor, Color backColor) =>
			new SkiaHatchBrush(style, foreColor, backColor);

		/// <summary>Real (Milestone E1, 2026-07-23) — see <see cref="SkiaPathGradientBrush"/>.</summary>
		public IPathGradientBrush CreatePathGradientBrush(IGraphicsPath path) => new SkiaPathGradientBrush(path);

		public IChartImage LoadImage(Stream stream) => new SkiaChartImage(SKBitmap.Decode(stream));

		/// <summary>
		/// Real (E1) on backends where <see cref="Image"/> can be constructed at all (Windows) — bridges
		/// via a PNG round-trip (no direct GDI+ Bitmap → SKBitmap pixel-buffer share is exposed by either
		/// API). Still unreachable on Linux, where <see cref="Image"/> itself cannot be constructed
		/// (see spike report) — <c>ImageLoader</c>'s <c>chart.Images</c>/<c>ResourceManager</c>/<c>WebRequest</c>/
		/// <c>File</c> pipeline stays GDI+-only regardless of this method's existence.
		/// </summary>
		public IChartImage WrapImage(Image image)
		{
			using MemoryStream stream = new MemoryStream();
			image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
			stream.Position = 0;
			return new SkiaChartImage(SKBitmap.Decode(stream));
		}

		public IChartFont CreateFont(string familyName, float sizeInPoints) => new SkiaChartFont(familyName, sizeInPoints);

		public IChartFont CreateFont(string familyName, float size, FontStyle style) => new SkiaChartFont(familyName, size, style);

		public IChartFont CreateFont(string familyName, float size, FontStyle style, GraphicsUnit unit) => new SkiaChartFont(familyName, size, style);

		public IChartFont DeriveFont(IChartFont prototype, FontStyle style) => new SkiaChartFont(prototype.FontFamilyName, prototype.SizeInPoints, style);

		public IChartFont DeriveFont(IChartFont prototype, float newSizeInPoints) => new SkiaChartFont(prototype.FontFamilyName, newSizeInPoints, prototype.Style);

		public IChartFont WrapFont(Font font) => new SkiaChartFont(font.FontFamily.Name, font.Size, font.Style);

		public ITextFormat CreateTextFormat() => new SkiaTextFormat();

		public ITextFormat CreateTypographicTextFormat() => new SkiaTextFormat();

		public ITextFormat CreateDefaultTextFormat() => new SkiaTextFormat();

		public IGraphicsPath CreatePath() => new SkiaGraphicsPath();

		/// <summary>Real (Milestone E2, 2026-07-23) — genuinely reachable (<c>CalloutAnnotation.SplitAtMarkers</c>/<c>DrawCloudCallout</c>); see <see cref="SkiaGraphicsPath.SkiaGraphicsPath(PointF[], byte[])"/>.</summary>
		public IGraphicsPath CreatePath(PointF[] points, byte[] types) => new SkiaGraphicsPath(points, types);

		public IClipRegion CreateRegion() => new SkiaClipRegion();

		public IClipRegion CreateRegion(RectangleF rect) => new SkiaClipRegion(rect);

		public IClipRegion CreateRegion(IGraphicsPath path) => new SkiaClipRegion(path);

		public IImageDrawOptions CreateImageDrawOptions() => new SkiaImageDrawOptions();
	}
}
