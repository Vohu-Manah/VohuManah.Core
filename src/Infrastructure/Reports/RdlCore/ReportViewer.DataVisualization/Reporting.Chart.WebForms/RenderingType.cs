namespace Reporting.Chart.WebForms
{
	internal enum RenderingType
	{
		Gdi,
		Svg,

		/// <summary>
		/// Milestone E2 (2026-07-23) — selects <see cref="SkiaChartGraphics"/> as
		/// <see cref="ChartRenderingEngine.RenderingObject"/>. Not wired into any production
		/// entry point yet (<c>ChartImage.GetImage</c>/<c>SaveImage</c> still hard-select
		/// <c>GdiRenderSurface</c>); reachable today only by setting
		/// <c>ChartPicture.chartGraph.ActiveRenderingType</c> directly and calling
		/// <c>ChartPicture.Paint(IRenderSurface, ...)</c> with a <see cref="Rendering.Skia.SkiaRenderSurface"/>.
		/// </summary>
		Skia
	}
}
