using System;
using System.Collections.Generic;

namespace ReportingServices.Rendering.RichText
{
	/// <summary>
	/// Reorders a paragraph's itemized runs from logical (reading/storage) order into visual
	/// (left-to-right draw) order - the run-level counterpart to the Unicode bidi algorithm's
	/// L2 rule (UAX #9), scoped to the two-level case this codebase's itemizers produce
	/// (a run is either the paragraph's base direction or the opposite one; no explicit
	/// directional-formatting-character nesting is itemized). Each run's own glyphs are
	/// already in visual order courtesy of HarfBuzz/SKShaper (which reverses RTL glyph order
	/// itself during shaping) - this class only reorders the *sequence of runs*, which is
	/// still in logical order after <see cref="UnicodeTextItemizer"/>/<see cref="UnicodeParagraphShaper"/>.
	/// </summary>
	internal static class BidiRunReorderer
	{
		/// <summary>
		/// Reorders <paramref name="items"/> in place into visual order. The paragraph's base
		/// direction is taken from the first item (mirrors UAX #9 P2/P3's "first strong
		/// character" heuristic) - a document/textbox-level direction override is not
		/// itemized today, so this is the best available signal.
		/// </summary>
		internal static void ReorderToVisualOrder<T>(IList<T> items, Func<T, bool> isRtl)
		{
			if (items == null || items.Count < 2)
			{
				return;
			}

			int baseLevel = isRtl(items[0]) ? 1 : 0;
			int[] levels = new int[items.Count];
			int maxLevel = baseLevel;
			int minOddLevel = int.MaxValue;
			for (int i = 0; i < items.Count; i++)
			{
				int itemDir = isRtl(items[i]) ? 1 : 0;
				levels[i] = (itemDir == baseLevel % 2) ? baseLevel : baseLevel + 1;
				if (levels[i] > maxLevel)
				{
					maxLevel = levels[i];
				}
				if (levels[i] % 2 == 1 && levels[i] < minOddLevel)
				{
					minOddLevel = levels[i];
				}
			}

			if (minOddLevel == int.MaxValue)
			{
				return;
			}

			for (int level = maxLevel; level >= minOddLevel; level--)
			{
				int start = -1;
				for (int i = 0; i <= items.Count; i++)
				{
					bool atOrAboveLevel = i < items.Count && levels[i] >= level;
					if (atOrAboveLevel && start < 0)
					{
						start = i;
					}
					else if (!atOrAboveLevel && start >= 0)
					{
						ReverseRange(items, start, i - 1);
						ReverseRange(levels, start, i - 1);
						start = -1;
					}
				}
			}
		}

		private static void ReverseRange<T>(IList<T> list, int start, int end)
		{
			while (start < end)
			{
				(list[start], list[end]) = (list[end], list[start]);
				start++;
				end--;
			}
		}
	}
}
