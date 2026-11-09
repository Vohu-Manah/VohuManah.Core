using System.Drawing;

namespace Reporting.Rendering
{
	/// <summary>
	/// Text layout options. Abstracts <see cref="System.Drawing.StringFormat"/>.
	/// Appendix A.3: 4 members + 2 presets (typographic / default). Tiny contract;
	/// L sizing is purely the 19-file spread. A backend may implement this as an
	/// immutable record behind the interface.
	/// </summary>
	internal interface ITextFormat : IRenderingResource
	{
		StringAlignment Alignment { get; set; }

		StringAlignment LineAlignment { get; set; }

		StringFormatFlags FormatFlags { get; set; }

		StringTrimming Trimming { get; set; }

		ITextFormat Clone();
	}
}
