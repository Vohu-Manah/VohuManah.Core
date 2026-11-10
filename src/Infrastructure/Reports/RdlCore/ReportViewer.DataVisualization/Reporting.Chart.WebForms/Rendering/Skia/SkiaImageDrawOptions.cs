using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Reporting.Rendering;

namespace Reporting.Chart.WebForms.Rendering.Skia
{
	/// <summary>
	/// <see cref="SetTransparentColor"/>/<see cref="SetWrapMode"/> are real (E1) — both are pure
	/// state consumed by <see cref="SkiaResourceFactory.CreateTextureBrush(IChartImage, RectangleF, IImageDrawOptions)"/>
	/// (colour-key via a per-pixel bitmap copy, wrap mode via <c>SKShaderTileMode</c>). The
	/// remaining members have no consumer yet on this backend (no <c>DrawImage(IChartImage,...)</c>
	/// overload is wired up) — still spike stubs.
	/// </summary>
	internal sealed class SkiaImageDrawOptions : IImageDrawOptions
	{
		internal Color? TransparentColor { get; private set; }

		internal WrapMode WrapMode { get; private set; } = WrapMode.Tile;

		public void SetColorRemap(Color from, Color to) => throw new NotImplementedException("Spike scope: not exercised by the sample scene.");

		public void SetTransparentColor(Color color) => TransparentColor = color;

		public void SetWrapMode(WrapMode mode) => WrapMode = mode;

		public void SetOpacity(float opacity) => throw new NotImplementedException("Spike scope: not exercised by the sample scene.");

		public void SetChannelScale(float red, float green, float blue, float alpha) => throw new NotImplementedException("Spike scope: not exercised by the sample scene.");

		public void Dispose()
		{
		}
	}
}
