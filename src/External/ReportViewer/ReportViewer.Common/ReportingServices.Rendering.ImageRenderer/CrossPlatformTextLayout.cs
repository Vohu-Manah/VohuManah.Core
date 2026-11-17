using ReportingServices.Rendering.RichText;

namespace ReportingServices.Rendering.ImageRenderer
{
	/// <summary>
	/// One piece of a wrapped line: a run of text drawn with a single style. Adjacent
	/// same-style fragments are merged so PDFWriter emits one Tj per style change, not
	/// per word. Produced by <see cref="ShapedStyledTextWrapper"/> (see
	/// ShapedTextLayout.cs) - the approximate-width word-wrapper this file originally
	/// held (SimpleTextWrapper/StyledTextWrapper/ApproximateTextMetrics) was replaced by
	/// real shaped-glyph-width wrapping once the P4 prototype pipeline was wired in
	/// (tasks/pdf-text-shaping-abstraction.md).
	/// </summary>
	internal readonly struct StyledLineFragment
	{
		internal readonly string Text;
		internal readonly ITextRunProps Style;

		internal StyledLineFragment(string text, ITextRunProps style)
		{
			Text = text;
			Style = style;
		}
	}
}
