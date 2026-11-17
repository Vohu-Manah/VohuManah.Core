using System.Drawing;

namespace Reporting.Rendering
{
	/// <summary>
	/// Font resource. Abstracts <see cref="System.Drawing.Font"/>.
	/// Appendix A.2: small immutable surface (family / size / style / unit); the wide
	/// blast radius is 31 files, not API complexity. Text MEASUREMENT is deliberately
	/// NOT here — it flows through <c>IChartRenderingEngine.MeasureString</c> so the
	/// backend (GDI+ / Skia) owns metrics.
	/// </summary>
	internal interface IChartFont : IRenderingResource
	{
		string FontFamilyName { get; }

		float SizeInPoints { get; }

		FontStyle Style { get; }

		GraphicsUnit Unit { get; }
	}
}
