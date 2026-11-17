using System.Drawing;
using Reporting.Rendering;

namespace Reporting.Chart.WebForms.Rendering.Gdi
{
	/// <summary>Milestone A2 adapter — wraps <see cref="System.Drawing.Image"/> behind <see cref="IChartImage"/>.</summary>
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
}
