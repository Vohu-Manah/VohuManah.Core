using System.Drawing;
using System.Numerics;
using Reporting.Rendering;

namespace Reporting.Gauge.WebForms.Rendering
{
	/// <summary>
	/// Gauge-engine counterpart of <c>Reporting.Chart.WebForms.Rendering.IClipRegion</c>
	/// (see tasks/gauge-gdi-type-abstraction.md Milestone A4). Abstracts
	/// <see cref="System.Drawing.Region"/>. Gauge-owned rather than shared/relocated — mirrors Chart's
	/// own finding that <c>IClipRegion</c> can't move to the shared <c>Reporting.Rendering</c>
	/// namespace, since <see cref="GetBounds"/>/<see cref="IsEmpty"/>/<see cref="IsInfinite"/> need a
	/// live <see cref="Graphics"/> (GDI+'s <c>Region.GetBounds(Graphics)</c> etc. require one) and the
	/// only thing that exposes one is each engine's own rendering-engine interface.
	/// </summary>
	internal interface IGaugeClipRegion : IRenderingResource
	{
		void Intersect(RectangleF rect);

		void Intersect(IGraphicsPath path);

		void Union(RectangleF rect);

		void Exclude(RectangleF rect);

		/// <summary>Abstracts <c>Region.Complement(GraphicsPath)</c> — updates this region to the portion of <paramref name="path"/> that does not intersect it.</summary>
		void Complement(IGraphicsPath path);

		void Xor(RectangleF rect);

		void MakeEmpty();

		void MakeInfinite();

		bool IsVisible(PointF point);

		void Transform(Matrix3x2 matrix);

		void Translate(float dx, float dy);

		/// <summary>Independent copy — abstracts <c>Region.Clone()</c>.</summary>
		IGaugeClipRegion Clone();

		RectangleF GetBounds(IGaugeRenderingEngine engine);

		bool IsEmpty(IGaugeRenderingEngine engine);

		bool IsInfinite(IGaugeRenderingEngine engine);
	}
}
