using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Reporting.Rendering;

namespace Reporting.Chart.WebForms.Rendering.Gdi
{
	/// <summary>Milestone A2 adapter — wraps <see cref="System.Drawing.Imaging.ImageAttributes"/> behind <see cref="IImageDrawOptions"/>.</summary>
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
			var matrix = new ColorMatrix { Matrix33 = opacity };
			NativeAttributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
		}

		public void SetChannelScale(float red, float green, float blue, float alpha)
		{
			var matrix = new ColorMatrix
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
