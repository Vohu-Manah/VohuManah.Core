using System.Collections.Generic;

namespace ReportingServices.Rendering.RichText
{
	/// <summary>
	/// Coarse Unicode-range script classification used only by <see cref="UnicodeTextItemizer"/>
	/// to decide item boundaries - not a replacement for Uniscribe's full script table
	/// (<see cref="ScriptProperties"/>), which this prototype does not attempt to model.
	/// </summary>
	internal enum UnicodeScriptKind
	{
		Common,
		Latin,
		Cyrillic,
		Greek,
		Hebrew,
		Arabic,
		Han,
		Thai,
		Devanagari,
		Hangul,
		Syriac,
		Thaana,
		NKo,
		Samaritan,
		Mandaic,
		Adlam,
		Other
	}

	/// <summary>One itemized run: a contiguous character range sharing one script/direction, with a constructed SCRIPT_ANALYSIS matching what Win32.ScriptItemize would have produced for it.</summary>
	internal readonly struct TextItem
	{
		internal readonly int CharPos;
		internal readonly int Length;
		internal readonly UnicodeScriptKind Script;
		internal readonly SCRIPT_ANALYSIS Analysis;

		internal TextItem(int charPos, int length, UnicodeScriptKind script, SCRIPT_ANALYSIS analysis)
		{
			CharPos = charPos;
			Length = length;
			Script = script;
			Analysis = analysis;
		}
	}

	/// <summary>
	/// Prototype "itemization" step for tasks/pdf-text-shaping-abstraction.md's P4 step 3
	/// (following HarfBuzzTextShaper's shaping-translation prototype): splits a string
	/// into script/direction runs the way <see cref="Paragraph.ScriptItemize"/> does via
	/// Win32's ScriptItemize, but using coarse Unicode code-point range checks instead of
	/// Uniscribe's script table. Produces a constructed <see cref="SCRIPT_ANALYSIS"/> per
	/// item via the existing <see cref="ScriptAnalysis"/>/<see cref="ScriptState"/>
	/// managed wrappers (no P/Invoke, no reflection - those wrappers already expose an
	/// internal encode path from plain fields back to the packed struct).
	///
	/// Honest scope: this is deliberately not a script-table port. It only distinguishes
	/// enough script buckets to (a) get direction (LTR/RTL) right for the common business-
	/// report case and (b) demonstrate that item boundaries can be produced without
	/// Uniscribe at all. <see cref="UnicodeScriptKind.Common"/> characters (whitespace,
	/// digits, ASCII punctuation) merge into whichever neighboring script run they're
	/// adjacent to (mirroring Uniscribe's own "common" script merge behavior) rather than
	/// forcing a new item per punctuation mark.
	///
	/// <see cref="UnicodeScriptKind.Thai"/>, <see cref="UnicodeScriptKind.Devanagari"/>,
	/// and <see cref="UnicodeScriptKind.Hangul"/> get their own item/script bucket (all
	/// correctly LTR) rather than falling into <see cref="UnicodeScriptKind.Other"/> - this
	/// itemizer still does not model their real shaping/reordering rules (e.g. Devanagari
	/// consonant-conjunct reordering, Thai's lack of word-boundary spacing), which is left
	/// to whatever shapes the run's glyphs (<see cref="HarfBuzzTextShaper"/>) rather than
	/// to itemization. <see cref="UnicodeScriptKind.Syriac"/> and
	/// <see cref="UnicodeScriptKind.Thaana"/> are itemized and correctly marked RTL,
	/// alongside <see cref="UnicodeScriptKind.Hebrew"/>/<see cref="UnicodeScriptKind.Arabic"/>.
	/// <see cref="UnicodeScriptKind.NKo"/>, <see cref="UnicodeScriptKind.Samaritan"/>, and
	/// <see cref="UnicodeScriptKind.Mandaic"/> (all BMP-resident, one UTF-16 char per
	/// codepoint) are itemized and correctly marked RTL the same way.
	/// <see cref="UnicodeScriptKind.Adlam"/> (U+1E900-U+1E95F) lives entirely outside the
	/// Basic Multilingual Plane, so each of its characters is a UTF-16 surrogate pair, not a
	/// single char. <see cref="ClassifyAt"/> is codepoint-aware for exactly this case - it
	/// detects a high/low surrogate pair, decodes the full codepoint, and classifies that
	/// pair as one two-char-wide unit - so Adlam is itemized and correctly marked RTL like
	/// the other RTL scripts above, rather than falling through to
	/// <see cref="UnicodeScriptKind.Other"/>/LTR. Any other supplementary-plane codepoint
	/// (e.g. emoji) still falls through to <see cref="UnicodeScriptKind.Other"/>, unchanged
	/// from before - only Adlam's range is recognized.
	/// <see cref="UnicodeScriptKind.Other"/> is everything still not explicitly bucketed -
	/// it gets its own item, treated as LTR.
	/// </summary>
	internal static class UnicodeTextItemizer
	{
		internal static List<TextItem> Itemize(string text)
		{
			var items = new List<TextItem>();
			if (string.IsNullOrEmpty(text))
			{
				return items;
			}

			int runStart = 0;
			UnicodeScriptKind runScript = ClassifyAt(text, 0, out int firstUnitLength);
			UnicodeScriptKind lastNonCommonScript = (runScript == UnicodeScriptKind.Common) ? UnicodeScriptKind.Latin : runScript;

			int i = firstUnitLength;
			while (i < text.Length)
			{
				UnicodeScriptKind charScript = ClassifyAt(text, i, out int unitLength);
				if (charScript == UnicodeScriptKind.Common)
				{
					// Common characters (spaces, digits, punctuation) never force a new
					// item - they merge into the run currently in progress, same as
					// Uniscribe treating "Common" script as compatible with any
					// neighboring script.
					i += unitLength;
					continue;
				}
				if (charScript == lastNonCommonScript)
				{
					i += unitLength;
					continue;
				}

				items.Add(BuildItem(runStart, i - runStart, lastNonCommonScript));
				runStart = i;
				lastNonCommonScript = charScript;
				i += unitLength;
			}

			items.Add(BuildItem(runStart, text.Length - runStart, lastNonCommonScript));
			return items;
		}

		/// <summary>Classifies the codepoint starting at <paramref name="index"/>, decoding a surrogate pair (needed for Adlam) rather than classifying each UTF-16 code unit independently. <paramref name="unitLength"/> is 2 for a decoded surrogate pair, 1 otherwise (including an unpaired/lone surrogate, classified defensively as a single code unit).</summary>
		private static UnicodeScriptKind ClassifyAt(string text, int index, out int unitLength)
		{
			char c = text[index];
			if (char.IsHighSurrogate(c) && index + 1 < text.Length && char.IsLowSurrogate(text[index + 1]))
			{
				unitLength = 2;
				int codePoint = char.ConvertToUtf32(c, text[index + 1]);
				if (codePoint is >= 0x1E900 and <= 0x1E95F)
				{
					return UnicodeScriptKind.Adlam;
				}
				return UnicodeScriptKind.Other;
			}
			unitLength = 1;
			return ClassifyChar(c);
		}

		private static TextItem BuildItem(int charPos, int length, UnicodeScriptKind script)
		{
			bool isRtl = IsRtlScript(script);
			var analysis = new ScriptAnalysis(0)
			{
				fRTL = isRtl ? 1 : 0,
				fLayoutRTL = isRtl ? 1 : 0,
				s = new ScriptState()
			};
			return new TextItem(charPos, length, script, analysis.GetAs_SCRIPT_ANALYSIS());
		}

		private static bool IsRtlScript(UnicodeScriptKind script)
		{
			return script == UnicodeScriptKind.Hebrew || script == UnicodeScriptKind.Arabic
				|| script == UnicodeScriptKind.Syriac || script == UnicodeScriptKind.Thaana
				|| script == UnicodeScriptKind.NKo || script == UnicodeScriptKind.Samaritan
				|| script == UnicodeScriptKind.Mandaic || script == UnicodeScriptKind.Adlam;
		}

		private static UnicodeScriptKind ClassifyChar(char c)
		{
			if (char.IsWhiteSpace(c) || char.IsDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c))
			{
				return UnicodeScriptKind.Common;
			}
			if (c is (>= 'A' and <= 'Z') or (>= 'a' and <= 'z') or (>= 'À' and <= 'ɏ'))
			{
				return UnicodeScriptKind.Latin;
			}
			if (c is >= 'Ͱ' and <= 'Ͽ')
			{
				return UnicodeScriptKind.Greek;
			}
			if (c is >= 'Ѐ' and <= 'ӿ')
			{
				return UnicodeScriptKind.Cyrillic;
			}
			if (c is >= '֐' and <= '׿')
			{
				return UnicodeScriptKind.Hebrew;
			}
			if (c is >= '؀' and <= 'ۿ')
			{
				return UnicodeScriptKind.Arabic;
			}
			if (c is >= 'ܐ' and <= 'ݏ')
			{
				return UnicodeScriptKind.Syriac;
			}
			if (c is >= 'ހ' and <= '޿')
			{
				return UnicodeScriptKind.Thaana;
			}
			if (c is >= '߀' and <= '߿')
			{
				return UnicodeScriptKind.NKo;
			}
			if (c is >= 'ࠀ' and <= '࠿')
			{
				return UnicodeScriptKind.Samaritan;
			}
			if (c is >= 'ࡀ' and <= '࡟')
			{
				return UnicodeScriptKind.Mandaic;
			}
			if (c is (>= '一' and <= '鿿') or (>= '぀' and <= 'ヿ'))
			{
				return UnicodeScriptKind.Han;
			}
			if (c is >= 'ऀ' and <= 'ॿ')
			{
				return UnicodeScriptKind.Devanagari;
			}
			if (c is >= '฀' and <= '๿')
			{
				return UnicodeScriptKind.Thai;
			}
			if (c is (>= 'ᄀ' and <= 'ᇿ') or (>= '가' and <= '힣') or (>= '㄰' and <= '㆏'))
			{
				return UnicodeScriptKind.Hangul;
			}
			return UnicodeScriptKind.Other;
		}
	}
}
