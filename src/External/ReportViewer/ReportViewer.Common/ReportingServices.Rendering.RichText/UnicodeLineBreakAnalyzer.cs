using System.Globalization;

namespace ReportingServices.Rendering.RichText
{
	/// <summary>
	/// Prototype line-break-attribute producer for tasks/pdf-text-shaping-abstraction.md's
	/// P4 step 3: produces a <see cref="SCRIPT_LOGATTR"/> per character the way
	/// <see cref="Paragraph.AnalyzeForBreakPositions"/> does via Win32.ScriptBreak, but
	/// using a hand-rolled break-class table instead of Uniscribe/ICU4N (this doc's
	/// phased plan explicitly names "ICU4N or a hand-rolled Unicode line-break
	/// implementation" as the two options; this is the latter).
	///
	/// Widened (2026-07-27) from a whitespace/hyphen/ideograph heuristic to a real
	/// approximation of the UAX #14 pair-based algorithm: every character is classified
	/// into a <see cref="LineBreakClass"/> (mirroring UAX #14's line-break property
	/// values, via <see cref="Classify"/>), and break opportunities are decided by
	/// applying a subset of UAX #14's rules (LB4-LB18 roughly) to each adjacent class
	/// pair - do not break before closing punctuation/quotes/percent even after a space
	/// or hyphen, do not break around glue characters (word joiner/NBSP, with the
	/// LB12a space/hyphen exception), keep CR+LF together as one mandatory break unit,
	/// keep digits/decimal separators together, etc.
	///
	/// Honest scope: this is NOT a certified-conformant UAX #14 implementation. It does
	/// not embed the full Unicode LineBreak.txt property table (thousands of codepoint
	/// ranges with many classes this doesn't model - AI/SG/XX/SA/CB/RI/EB/EM/ZWJ/JL/JV/
	/// JT tailoring, etc.) - unclassified characters fall back to <see cref="LineBreakClass.AL"/>
	/// (ordinary alphabetic), and Hangul syllables are folded into <see cref="LineBreakClass.ID"/>
	/// rather than given their own JL/JV/JT/H2/H3 treatment. There is no locale-specific
	/// tailoring (e.g. Japanese kinsoku shori). <see cref="SCRIPT_LOGATTR"/> itself only
	/// has two bits (fSoftBreak/fWhiteSpace), so UAX #14's distinction between "mandatory"
	/// and "optional" breaks can't be surfaced separately here either - both still map to
	/// fSoftBreak, same as before this widening.
	///
	/// Narrowed further (2026-07-27, next day): fullwidth '！'/'？'/'，'/'：'/'；' (U+FF01/FF1F/FF0C/FF1A/FF1B) -
	/// as common in Chinese/Japanese text as their halfwidth ASCII counterparts - now classify the same
	/// way those do, instead of falling through to AL. A bare '?' (previously missing from the EX case
	/// despite '!' already being there) was fixed alongside it. Two other named gaps above (locale-specific
	/// kinsoku-shori tailoring, and giving Hangul conjoining jamo their own JL/JV/JT classes instead of
	/// folding them into AL) were investigated and intentionally left alone: decomposed Hangul jamo
	/// sequences already don't get spurious internal breaks today (they fall to AL, which - like any other
	/// ordinary-alphabetic run - never breaks mid-run under this analyzer's default-deny model), so adding
	/// dedicated JL/JV/JT classes would add real complexity for no verifiable behavior change; and kinsoku
	/// shori's core mechanics (don't break before closing punctuation/quotes, don't break after opening
	/// punctuation) are already covered by the existing CL/OP/QU/NS rules above - full locale tailoring
	/// beyond that would need the actual Unicode LineBreak.txt table this class deliberately doesn't embed.
	/// </summary>
	internal static class UnicodeLineBreakAnalyzer
	{
		internal static SCRIPT_LOGATTR[] Analyze(string text)
		{
			LineBreakClass[] classes = new LineBreakClass[text.Length];
			for (int i = 0; i < text.Length; i++)
			{
				classes[i] = Classify(text[i]);
			}

			SCRIPT_LOGATTR[] result = new SCRIPT_LOGATTR[text.Length];
			for (int i = 0; i < text.Length; i++)
			{
				bool isWhiteSpace = classes[i] == LineBreakClass.SP;
				bool isSoftBreak = i == 0 || CanBreakBefore(classes[i - 1], classes[i]);
				result[i] = SCRIPT_LOGATTR.FromFlags(isSoftBreak, isWhiteSpace);
			}
			return result;
		}

		private static bool CanBreakBefore(LineBreakClass prev, LineBreakClass cur)
		{
			// LB5: CR+LF is a single mandatory-break unit - never split it.
			if (prev == LineBreakClass.CR && cur == LineBreakClass.LF)
			{
				return false;
			}

			// LB4/LB5: always break after a mandatory-break character.
			if (IsMandatoryBreak(prev))
			{
				return true;
			}

			// LB6: never break immediately before a mandatory-break character - the
			// break happens after it instead, not before.
			if (IsMandatoryBreak(cur))
			{
				return false;
			}

			// LB12/LB12a: glue characters (word joiner, NBSP) never allow a break after
			// them; before them, only when preceded by whitespace or a hyphen-like break.
			if (prev == LineBreakClass.GL || prev == LineBreakClass.WJ)
			{
				return false;
			}
			if (cur == LineBreakClass.GL || cur == LineBreakClass.WJ)
			{
				return prev == LineBreakClass.SP || prev == LineBreakClass.BA || prev == LineBreakClass.HY;
			}

			// LB13/LB9: never break before closing punctuation, quotes, non-starters,
			// combining marks, or numeric separators/suffixes - even after a space or a
			// hyphen, unlike ordinary letters.
			if (cur == LineBreakClass.CL || cur == LineBreakClass.EX || cur == LineBreakClass.IS
				|| cur == LineBreakClass.SY || cur == LineBreakClass.NS || cur == LineBreakClass.CM
				|| cur == LineBreakClass.QU)
			{
				return false;
			}

			// Quotes/opening brackets never allow a break right after them either - they
			// stick to whatever they introduce.
			if (prev == LineBreakClass.QU || prev == LineBreakClass.OP)
			{
				return false;
			}

			// LB25 (numeric context): keep a run of digits, and digits separated by
			// '.'/',' or '/', together; currency prefixes stick to the number that
			// follows, and a postfix like '%' sticks to the number before it.
			if (prev == LineBreakClass.NU && (cur == LineBreakClass.NU || cur == LineBreakClass.IS || cur == LineBreakClass.SY))
			{
				return false;
			}
			if (prev == LineBreakClass.PR && cur == LineBreakClass.NU)
			{
				return false;
			}
			if (prev == LineBreakClass.NU && cur == LineBreakClass.PO)
			{
				return false;
			}

			// LB17: never break between two consecutive B2 (e.g. em dash em dash).
			if (prev == LineBreakClass.B2)
			{
				return cur != LineBreakClass.B2;
			}

			// LB18: break after any run of whitespace.
			if (prev == LineBreakClass.SP)
			{
				return true;
			}

			// Hyphen-like characters (HY/BA) allow a break right after them (mid-word
			// hyphenation, e.g. "well-known" can wrap after "well-").
			if (prev == LineBreakClass.HY || prev == LineBreakClass.BA)
			{
				return true;
			}

			// CJK ideographs/kana/Hangul syllables permit a break between almost every
			// adjacent pair under UAX #14's ID/H2/H3 classes, unlike space-delimited
			// scripts which only break at whitespace/hyphen boundaries.
			if (prev == LineBreakClass.ID && cur == LineBreakClass.ID)
			{
				return true;
			}

			return false;
		}

		private static bool IsMandatoryBreak(LineBreakClass c)
		{
			return c == LineBreakClass.BK || c == LineBreakClass.CR || c == LineBreakClass.LF || c == LineBreakClass.NL;
		}

		/// <summary>
		/// A coarse approximation of UAX #14's line-break property values - not the full
		/// Unicode LineBreak.txt table, just the classes this analyzer's rules act on.
		/// </summary>
		private enum LineBreakClass
		{
			AL,  // Ordinary alphabetic/default.
			BK,  // Mandatory break (form feed, line/paragraph separator).
			CR,  // Carriage return.
			LF,  // Line feed.
			NL,  // Next line.
			SP,  // Space.
			GL,  // Non-breaking glue (NBSP, narrow NBSP, figure space).
			WJ,  // Word joiner / zero-width no-break space.
			QU,  // Quotation mark.
			OP,  // Opening punctuation.
			CL,  // Closing punctuation.
			EX,  // Exclamation/question mark.
			IS,  // Infix numeric/general separator (',', ':', ';').
			SY,  // Symbol allowing a break after ('/').
			HY,  // Hyphen ('-').
			BA,  // Break-after (soft hyphen, hyphen-2010).
			B2,  // Break on either side, but not between two of its own kind (dashes).
			PR,  // Numeric prefix (currency symbols).
			PO,  // Numeric postfix ('%').
			NU,  // Digit.
			CM,  // Combining mark.
			NS,  // Non-starter (can't begin a line - CJK small kana, ideographic punctuation).
			ID,  // Ideograph (CJK/kana/Hangul) - breakable between adjacent instances.
		}

		private static LineBreakClass Classify(char c)
		{
			switch (c)
			{
				case '\r': return LineBreakClass.CR;
				case '\n': return LineBreakClass.LF;
				case '\u0085': return LineBreakClass.NL;
				case '\u000B':
				case '\u000C':
				case '\u2028':
				case '\u2029':
					return LineBreakClass.BK;
				case '\u200B': // Zero-width space behaves like a break-after point.
					return LineBreakClass.BA;
				case '⁠': // Word joiner.
				case '﻿': // Zero-width no-break space.
					return LineBreakClass.WJ;
				case ' ': // No-break space.
				case ' ': // Narrow no-break space.
				case ' ': // Figure space.
					return LineBreakClass.GL;
				case '"': case '\'':
				case '«': case '»':
				case '‘': case '’': case '‚': case '„':
				case '“': case '”':
				case '‹': case '›':
					return LineBreakClass.QU;
				case '(': case '[': case '{':
				case '〈': case '《': case '「': case '『': case '【':
				case '（': case '［': case '｛':
					return LineBreakClass.OP;
				case ')': case ']': case '}':
				case '〉': case '》': case '」': case '』': case '】':
				case '）': case '］': case '｝':
					return LineBreakClass.CL;
				case '!': case '?': // A bare '?' was previously missing here (fell through to AL) despite '!' already being classified - both are ordinary EX-class punctuation per UAX #14.
				case '‼': case '⁉':
				case '！': case '？': // Fullwidth forms (U+FF01/FF1F) - as common in Chinese/Japanese text as their halfwidth ASCII counterparts, and should classify the same way.
					return LineBreakClass.EX;
				case ',': case ':': case ';':
				case '，': case '：': case '；': // Fullwidth forms (U+FF0C/FF1A/FF1B).
					return LineBreakClass.IS;
				case '/':
					return LineBreakClass.SY;
				case '-':
					return LineBreakClass.HY;
				case '­': // Soft hyphen.
				case '‐': // Hyphen.
					return LineBreakClass.BA;
				case '–': // En dash.
				case '—': // Em dash.
					return LineBreakClass.B2;
				case '$': case '#':
				case '£': case '¥': case '€':
					return LineBreakClass.PR;
				case '%': case '‰':
					return LineBreakClass.PO;
				case '、': case '。': // Ideographic comma/full stop.
				case '・': case 'ー': // Katakana middle dot/prolonged sound mark.
				case '々': // Ideographic iteration mark.
				case '…': // Horizontal ellipsis.
					return LineBreakClass.NS;
			}

			if (char.IsWhiteSpace(c))
			{
				return LineBreakClass.SP;
			}
			if (char.IsDigit(c))
			{
				return LineBreakClass.NU;
			}
			if (IsWrappableIdeograph(c))
			{
				return LineBreakClass.ID;
			}

			UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
			if (category == UnicodeCategory.NonSpacingMark || category == UnicodeCategory.SpacingCombiningMark
				|| category == UnicodeCategory.EnclosingMark)
			{
				return LineBreakClass.CM;
			}

			return LineBreakClass.AL;
		}

		/// <summary>
		/// CJK Unified Ideographs (U+4E00-U+9FFF), Hiragana/Katakana (U+3040-U+30FF), and
		/// Hangul syllables (U+AC00-U+D7A3) - scripts whose UAX #14 break classes
		/// (ID/H2/H3) permit a break between almost every adjacent character pair, since
		/// (unlike Latin-style scripts) they aren't conventionally space-delimited.
		/// </summary>
		private static bool IsWrappableIdeograph(char c)
		{
			return (c >= '一' && c <= '鿿') || (c >= '぀' && c <= 'ヿ') || (c >= '가' && c <= '힣');
		}
	}
}
