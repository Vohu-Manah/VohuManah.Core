namespace Reporting.Rendering
{
	/// <summary>
	/// A decoded image usable as a texture/draw source. Abstracts <c>System.Drawing.Image</c>.
	/// Relocated from <c>Reporting.Chart.WebForms.Rendering</c> to this shared namespace
	/// (verified portable — no Chart-specific dependencies — during the Gauge engine's
	/// GetTextureBrush prerequisite, see tasks/gauge-gdi-type-abstraction.md Milestone B).
	/// </summary>
	internal interface IChartImage : IRenderingResource
	{
		int Width { get; }

		int Height { get; }
	}
}
