using System.Collections.Generic;

namespace ReportingServices.Rendering.RichText
{
	/// <summary>
	/// One itemized-and-shaped run within a paragraph: the composed output of
	/// <see cref="UnicodeTextItemizer"/> (item boundaries + script/direction),
	/// <see cref="UnicodeLineBreakAnalyzer"/> (this item's slice of the paragraph's
	/// per-character break attributes), and <see cref="HarfBuzzTextShaper"/> (this
	/// item's shaped glyph data) - the same three ingredients <see cref="Paragraph.ScriptItemize"/>/
	/// <see cref="Paragraph.AnalyzeForBreakPositions"/>/<see cref="TextRun.ShapeAndPlace"/>
	/// assemble today via Uniscribe, for one run.
	/// </summary>
	internal readonly struct ShapedRunItem
	{
		internal readonly int CharPos;
		internal readonly int Length;
		internal readonly UnicodeScriptKind Script;
		internal readonly SCRIPT_ANALYSIS Analysis;
		internal readonly SCRIPT_LOGATTR[] ScriptLogAttr;
		internal readonly GlyphData GlyphData;

		internal ShapedRunItem(int charPos, int length, UnicodeScriptKind script, SCRIPT_ANALYSIS analysis, SCRIPT_LOGATTR[] scriptLogAttr, GlyphData glyphData)
		{
			CharPos = charPos;
			Length = length;
			Script = script;
			Analysis = analysis;
			ScriptLogAttr = scriptLogAttr;
			GlyphData = glyphData;
		}
	}

	/// <summary>
	/// Composes <see cref="UnicodeTextItemizer"/>, <see cref="UnicodeLineBreakAnalyzer"/>,
	/// and <see cref="HarfBuzzTextShaper"/> into a single per-paragraph pipeline - the
	/// integration step beyond each prototype's own isolated tests (tasks/pdf-text-
	/// shaping-abstraction.md, P4 step 3). Confirms the three pieces actually fit
	/// together (item boundaries slice the line-break array and the shaped text
	/// consistently, with no gaps or overlaps) before any of this touches production
	/// code.
	///
	/// Still NOT wired into <see cref="LineBreaker"/>/<see cref="Paragraph"/>/<see cref="TextRun"/>
	/// - the real, Win32-HDC-threaded RichText call graph (61 call sites, see this doc's
	/// step-3 scoping note) is untouched.
	///
	/// It IS wired into <see cref="ReportingServices.Rendering.ImageRenderer.PDFWriter"/>'s
	/// cross-platform text path: <see cref="ReportingServices.Rendering.ImageRenderer.ShapedTextMetrics"/>
	/// consumes this pipeline's per-character widths and soft-break flags to decide
	/// where to word-wrap and how wide to draw decoration rectangles (order-independent,
	/// since it re-indexes by each item's absolute <see cref="ShapedRunItem.CharPos"/>), and
	/// <c>PDFWriter.WriteCompositeText</c> draws the returned items in list order to emit
	/// glyph-indexed Tj output against a real embedded font. The returned list is in visual
	/// (left-to-right draw) order, not logical (storage/reading) order - see
	/// <see cref="BidiRunReorderer"/>.
	/// </summary>
	internal static class UnicodeParagraphShaper
	{
		internal static List<ShapedRunItem> Shape(string text, SkiaCachedFont font)
		{
			var result = new List<ShapedRunItem>();
			if (string.IsNullOrEmpty(text))
			{
				return result;
			}

			SCRIPT_LOGATTR[] lineBreakAttrs = UnicodeLineBreakAnalyzer.Analyze(text);
			List<TextItem> items = UnicodeTextItemizer.Itemize(text);

			foreach (TextItem item in items)
			{
				string itemText = text.Substring(item.CharPos, item.Length);
				SCRIPT_LOGATTR[] itemLogAttr = new SCRIPT_LOGATTR[item.Length];
				System.Array.Copy(lineBreakAttrs, item.CharPos, itemLogAttr, 0, item.Length);

				GlyphData glyphData = HarfBuzzTextShaper.Shape(itemText, font);

				result.Add(new ShapedRunItem(item.CharPos, item.Length, item.Script, item.Analysis, itemLogAttr, glyphData));
			}

			BidiRunReorderer.ReorderToVisualOrder(result, item => (item.Analysis.word1 & (1 << 10)) != 0);

			return result;
		}
	}
}
