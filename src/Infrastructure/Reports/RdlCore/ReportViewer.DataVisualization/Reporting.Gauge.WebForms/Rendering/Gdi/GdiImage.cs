using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Reporting.Rendering;

namespace Reporting.Gauge.WebForms.Rendering.Gdi
{
	/// <summary>
	/// GetTextureBrush prerequisite adapter (see tasks/gauge-gdi-type-abstraction.md Milestone B) —
	/// wraps <see cref="System.Drawing.Image"/> behind <see cref="IChartImage"/>. Gauge-owned,
	/// separate from the Chart engine's identically-shaped <c>GdiChartImage</c> per the migration's
	/// decoupled-adapter design (Milestone A2 decision).
	/// </summary>
	internal sealed class GdiChartImage : IChartImage
	{
		internal Image NativeImage { get; }

		internal GdiChartImage(Image image)
		{
			NativeImage = image;
		}

		public int Width => NativeImage.Width;

		public int Height => NativeImage.Height;

		public void Dispose() => NativeImage.Dispose();
	}

	/// <summary>
	/// GetTextureBrush prerequisite adapter (see tasks/gauge-gdi-type-abstraction.md Milestone B) —
	/// wraps <see cref="System.Drawing.Imaging.ImageAttributes"/> behind <see cref="IImageDrawOptions"/>.
	/// Gauge-owned, separate from the Chart engine's identically-shaped <c>GdiImageDrawOptions</c>.
	/// </summary>
	internal sealed class GdiImageDrawOptions : IImageDrawOptions
	{
		internal ImageAttributes NativeAttributes { get; } = new ImageAttributes();

		public void SetColorRemap(Color from, Color to)
		{
			NativeAttributes.SetRemapTable(new[] { new ColorMap { OldColor = from, NewColor = to } });
		}

		public void SetTransparentColor(Color color) => NativeAttributes.SetColorKey(color, color, ColorAdjustType.Default);

		public void SetWrapMode(WrapMode mode) => NativeAttributes.SetWrapMode(mode);

		public void SetOpacity(float opacity)
		{
			ColorMatrix matrix = new ColorMatrix { Matrix33 = opacity };
			NativeAttributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
		}

		public void SetChannelScale(float red, float green, float blue, float alpha)
		{
			ColorMatrix matrix = new ColorMatrix
			{
				Matrix00 = red,
				Matrix11 = green,
				Matrix22 = blue,
				Matrix33 = alpha
			};
			NativeAttributes.SetColorMatrix(matrix);
		}

		public void Dispose() => NativeAttributes.Dispose();
	}
}
