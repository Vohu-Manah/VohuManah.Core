using System;
using SkiaSharp;
using SkiaSharp.HarfBuzz;

namespace ReportingServices.Rendering.RichText
{
	/// <summary>
	/// Prototype "shaping layer" for tasks/pdf-text-shaping-abstraction.md's P4 step 3:
	/// shapes a run of text with HarfBuzzSharp (via <see cref="SkiaCachedFont.Shaper"/>,
	/// the official <see cref="SkiaSharp.HarfBuzz.SKShaper"/> integration) and translates
	/// the result into the same <see cref="GlyphData"/>/<see cref="GlyphShapeData"/>/
	/// <see cref="ABC"/>/<see cref="GOFFSET"/> shapes that <see cref="TextRun.ShapeAndPlace"/>/
	/// <see cref="TextRun.TextScriptPlace"/> already produce from Win32's
	/// ScriptShape/ScriptPlace - so a future increment could swap the producer without
	/// changing what <see cref="LineBreaker"/>/<see cref="TextBox"/> consume.
	///
	/// Deliberately NOT wired into <see cref="TextRun"/>/<see cref="FontCache"/> yet -
	/// this is an exploratory prototype (per the "explore the depth of a feature, keep it
	/// if it works, discard it if not" approach) proving the translation itself is
	/// feasible before touching the much larger, riskier production call graph (61
	/// <c>Win32DCSafeHandle</c> call sites across `TextRun`/`LineBreaker`/`TextBox`/
	/// `TextLine`, per this doc's step-3 scoping note).
	///
	/// Scope, honestly: covers plain LTR, non-ligated-cluster-reordering text only -
	/// the "realistic majority case" this doc's phased plan calls out (Latin/Cyrillic/
	/// Greek/numbers/punctuation). What's NOT attempted here:
	/// - <see cref="SCRIPT_VISATTR"/> is left zeroed. This codebase never reads its bits
	///   itself (grepped: every reference just passes the array opaquely to further
	///   Win32 calls this prototype doesn't call), so zeroing is safe for this
	///   prototype's own contract, but a real port feeding these into actual Uniscribe
	///   calls (e.g. via a mixed old/new pipeline) would need real cluster-start/
	///   diacritic/RTL flags.
	/// - Per-character cluster mapping assumes <see cref="SKShaper.Result.Clusters"/>
	///   indices are non-decreasing glyph-to-glyph (true for LTR text with no glyph
	///   reordering; false for RTL and some complex-script ligature/reordering cases).
	/// - Fallback-font retry itself lives in <see cref="TextRun.ShapeAndPlaceCrossPlatform"/>/
	///   <see cref="FontCache.GetFallbackFontCrossPlatform"/> (mirroring <see cref="FontCache.GetFallbackFont"/>
	///   /<see cref="TextRun.ShapeAndPlace"/>'s .notdef retry loop) - this class's own job is
	///   only to report the first missing glyph's source codepoint via the
	///   <see cref="Shape(string, SkiaCachedFont, out int)"/> overload, not to retry itself.
	/// - No bidi reordering, no line-break/soft-break flag production (<see cref="SCRIPT_LOGATTR"/>
	///   is not touched by this class at all - that's <see cref="LineBreaker"/>'s
	///   ScriptBreak-derived data, a separate concern from shaping).
	/// - GOFFSET (per-glyph pen adjustment separate from advance, in Uniscribe's model)
	///   is always zero here: <see cref="SKShaper.Result.Points"/> already gives final
	///   cumulative pen positions with any such adjustment baked in, so this translation
	///   derives per-glyph advances from consecutive point deltas rather than reproducing
	///   Uniscribe's separate advance+offset model exactly.
	/// </summary>
	internal static class HarfBuzzTextShaper
	{
		internal static GlyphData Shape(string text, SkiaCachedFont font)
		{
			return Shape(text, font, out _);
		}

		/// <summary>
		/// Same as <see cref="Shape(string, SkiaCachedFont)"/>, but also reports the source
		/// Unicode codepoint of the first character whose glyph came back as glyph id 0
		/// (HarfBuzz's ".notdef" - the font has no real glyph for it) via
		/// <paramref name="firstMissingGlyphCodepoint"/> (-1 if every character shaped to a
		/// real glyph). Whitespace/control characters are never reported even if their glyph
		/// id happens to be 0, since a missing glyph there isn't visually meaningful and
		/// shouldn't trigger a fallback-font retry on its own.
		/// </summary>
		internal static GlyphData Shape(string text, SkiaCachedFont font, out int firstMissingGlyphCodepoint)
		{
			firstMissingGlyphCodepoint = -1;
			if (font == null)
			{
				throw new ArgumentNullException(nameof(font));
			}
			if (string.IsNullOrEmpty(text))
			{
				GlyphShapeData emptyShapeData = new GlyphShapeData(0, 0);
				return new GlyphData(emptyShapeData);
			}

			SKShaper.Result shaped = font.Shaper.Shape(text, font.Font);
			SKPoint[] points = shaped.Points;
			uint[] clusters = shaped.Clusters;
			int glyphCount = points.Length;

			GlyphShapeData glyphShapeData = new GlyphShapeData(Math.Max(glyphCount, 1), text.Length)
			{
				GlyphCount = glyphCount
			};

			for (int g = 0; g < glyphCount; g++)
			{
				glyphShapeData.Glyphs[g] = unchecked((short)shaped.Codepoints[g]);

				int clusterStart = (int)clusters[g];
				int clusterEnd = (g + 1 < glyphCount) ? (int)clusters[g + 1] : text.Length;
				for (int c = clusterStart; c < clusterEnd && c < text.Length; c++)
				{
					glyphShapeData.Clusters[c] = (short)g;
				}

				if (firstMissingGlyphCodepoint < 0 && shaped.Codepoints[g] == 0 && clusterStart < text.Length && !char.IsWhiteSpace(text[clusterStart]) && !char.IsControl(text[clusterStart]))
				{
					firstMissingGlyphCodepoint = (char.IsHighSurrogate(text[clusterStart]) && clusterStart + 1 < text.Length && char.IsLowSurrogate(text[clusterStart + 1]))
						? char.ConvertToUtf32(text[clusterStart], text[clusterStart + 1])
						: text[clusterStart];
				}
			}
			glyphShapeData.TrimToGlyphCount();

			GlyphData glyphData = new GlyphData(glyphShapeData);
			int totalAdvance = 0;
			for (int g = 0; g < glyphCount; g++)
			{
				float nextX = (g + 1 < glyphCount) ? points[g + 1].X : shaped.Width;
				int advance = (int)MathF.Round(nextX - points[g].X);
				glyphData.RawAdvances[g] = advance;
				glyphData.RawGOffsets[g] = default;
				totalAdvance += advance;
			}
			glyphData.ABC = new ABC
			{
				abcA = 0,
				abcB = (uint)Math.Max(totalAdvance, 0),
				abcC = 0
			};

			return glyphData;
		}
	}
}
