using System.Runtime.InteropServices;
using Reporting.Chart.WebForms.Rendering.Gdi;
using Reporting.Chart.WebForms.Rendering.Skia;

namespace Reporting.Chart.WebForms.Rendering
{
	/// <summary>
	/// Milestone F: selects the Gdi or Skia backend by platform, mirroring the Excel renderer's
	/// <c>ImageProviderFactory</c> pattern. Centralizes the one platform check both
	/// <see cref="ChartPicture.renderSurfaceFactory"/> and <see cref="ChartPicture.chartGraph"/>
	/// need to agree on — see chart-gdi-type-abstraction.md's Milestone F notes for why they
	/// must be selected together (a <see cref="SkiaRenderSurface"/> fed to a Gdi-backed
	/// <see cref="ChartGraphics"/>, or vice versa, is a cross-backend mismatch).
	/// </summary>
	internal static class ChartRenderingBackendFactory
	{
		private static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

		internal static IRenderSurfaceFactory CreateRenderSurfaceFactory() =>
			IsWindows ? new GdiRenderSurfaceFactory() : new SkiaRenderSurfaceFactory();

		internal static ChartGraphics CreateChartGraphics(CommonElements common) =>
			IsWindows
				? new ChartGraphics(common)
				: new ChartGraphics(common, new SkiaResourceFactory()) { ActiveRenderingType = RenderingType.Skia };
	}
}
