using System;
using System.Collections.Generic;

namespace ReportingServices.Rendering.ImageRenderer
{
	/// <summary>
	/// Reduces an embedded TrueType-outline (glyf/loca) font's file size to only the glyphs
	/// actually used in the rendered document, without renumbering glyph ids - so nothing
	/// else already emitted (glyph-indexed Tj hex strings, CIDToGIDMap /Identity, hmtx-
	/// derived /W widths in the PDF font dictionary) needs to change. Unlike a full font
	/// subsetter (which renumbers glyph ids to a dense, compact range), this only zeroes the
	/// outline data of glyphs never referenced - every other table, and every glyph id
	/// already baked into earlier PDF output, is untouched. This still captures most of a
	/// real subset's size win for a typical report (a handful of glyphs used out of a large
	/// multi-script font's thousands), at a much smaller implementation/risk cost than full
	/// glyph-id renumbering.
	///
	/// Honest scope: only TrueType-outline (glyf/loca) fonts are supported - CFF-flavored
	/// OpenType fonts are handled by the sibling <see cref="CffGlyphSubsetter"/>, and
	/// TrueType Collection ('ttcf') fonts are left whole-file-embedded (<see cref="TrySubset"/>
	/// returns false) - a collection would need its own member-font selection/remapping,
	/// out of scope for either subsetter. Any parse surprise falls back the same way -
	/// this never risks shipping a corrupt embedded font over a slightly larger correct one.
	/// </summary>
	internal static class TrueTypeGlyphSubsetter
	{
		private const uint TagGlyf = 0x676C7966u;

		private const uint TagLoca = 0x6C6F6361u;

		private const uint TagHead = 0x68656164u;

		private const uint TagMaxp = 0x6D617870u;

		private const int CompositeArgsAreWords = 0x0001;

		private const int CompositeWeHaveAScale = 0x0008;

		private const int CompositeMoreComponents = 0x0020;

		private const int CompositeWeHaveAnXAndYScale = 0x0040;

		private const int CompositeWeHaveATwoByTwo = 0x0080;

		internal static bool TrySubset(byte[] fontData, IEnumerable<ushort> usedGlyphIds, out byte[] subsetted)
		{
			try
			{
				return TrySubsetCore(fontData, usedGlyphIds, out subsetted);
			}
			catch (Exception)
			{
				subsetted = null;
				return false;
			}
		}

		private static bool TrySubsetCore(byte[] fontData, IEnumerable<ushort> usedGlyphIds, out byte[] subsetted)
		{
			subsetted = null;
			if (SfntBinaryUtils.DetectOutlineFormat(fontData) != SfntOutlineFormat.TrueType)
			{
				return false;
			}
			if (!SfntBinaryUtils.TryReadTableDirectory(fontData, out ushort numTables, out Dictionary<uint, SfntTableEntry> directory))
			{
				return false;
			}

			if (!directory.TryGetValue(TagGlyf, out SfntTableEntry glyfEntry) ||
				!directory.TryGetValue(TagLoca, out SfntTableEntry locaEntry) ||
				!directory.TryGetValue(TagHead, out SfntTableEntry headEntry) ||
				!directory.TryGetValue(TagMaxp, out SfntTableEntry maxpEntry))
			{
				return false;
			}

			short indexToLocFormat = (short)SfntBinaryUtils.ReadUInt16BE(fontData, headEntry.Offset + 50);
			if (indexToLocFormat != 0 && indexToLocFormat != 1)
			{
				return false;
			}
			ushort numGlyphs = SfntBinaryUtils.ReadUInt16BE(fontData, maxpEntry.Offset + 4);

			int[] loca = ReadLoca(fontData, locaEntry.Offset, numGlyphs, indexToLocFormat);
			if (loca == null)
			{
				return false;
			}

			HashSet<ushort> keep = ResolveKeptGlyphs(fontData, glyfEntry, loca, numGlyphs, usedGlyphIds);

			BuildGlyfAndLoca(fontData, glyfEntry, loca, numGlyphs, keep, out byte[] newGlyfTable, out int[] newLoca);
			byte[] newLocaTable = WriteLoca(newLoca, indexToLocFormat);

			subsetted = SfntBinaryUtils.RebuildFont(fontData, numTables, new Dictionary<uint, byte[]>
			{
				[TagGlyf] = newGlyfTable,
				[TagLoca] = newLocaTable
			});
			return true;
		}

		private static HashSet<ushort> ResolveKeptGlyphs(byte[] fontData, SfntTableEntry glyfEntry, int[] loca, ushort numGlyphs, IEnumerable<ushort> usedGlyphIds)
		{
			var keep = new HashSet<ushort> { 0 }; // .notdef is always required
			var toVisit = new Queue<ushort>();
			toVisit.Enqueue(0);
			foreach (ushort glyphId in usedGlyphIds)
			{
				if (glyphId < numGlyphs && keep.Add(glyphId))
				{
					toVisit.Enqueue(glyphId);
				}
			}

			while (toVisit.Count > 0)
			{
				ushort glyphId = toVisit.Dequeue();
				int start = loca[glyphId];
				int end = loca[glyphId + 1];
				if (end <= start)
				{
					continue; // empty glyph (e.g. space) - no outline, no component references
				}
				foreach (ushort component in GetCompositeComponents(fontData, glyfEntry.Offset + start, end - start))
				{
					if (component < numGlyphs && keep.Add(component))
					{
						toVisit.Enqueue(component);
					}
				}
			}
			return keep;
		}

		private static void BuildGlyfAndLoca(byte[] fontData, SfntTableEntry glyfEntry, int[] loca, ushort numGlyphs, HashSet<ushort> keep, out byte[] newGlyfTable, out int[] newLoca)
		{
			var newGlyf = new List<byte>(glyfEntry.Length);
			newLoca = new int[numGlyphs + 1];
			for (int glyphId = 0; glyphId < numGlyphs; glyphId++)
			{
				newLoca[glyphId] = newGlyf.Count;
				if (!keep.Contains((ushort)glyphId))
				{
					continue;
				}
				int start = loca[glyphId];
				int end = loca[glyphId + 1];
				for (int b = glyfEntry.Offset + start; b < glyfEntry.Offset + end; b++)
				{
					newGlyf.Add(fontData[b]);
				}
			}
			newLoca[numGlyphs] = newGlyf.Count;
			newGlyfTable = newGlyf.ToArray();
		}

		/// <summary>Simple glyphs (numberOfContours &gt;= 0) reference no other glyph. Composite glyphs (numberOfContours &lt; 0) are a chain of component records, each naming a component glyph id.</summary>
		private static IEnumerable<ushort> GetCompositeComponents(byte[] fontData, int glyphOffset, int glyphLength)
		{
			if (glyphLength < 10)
			{
				yield break;
			}
			short numberOfContours = (short)SfntBinaryUtils.ReadUInt16BE(fontData, glyphOffset);
			if (numberOfContours >= 0)
			{
				yield break;
			}

			int pos = glyphOffset + 10; // numberOfContours(2) + xMin/yMin/xMax/yMax(2 each)
			int end = glyphOffset + glyphLength;
			while (pos + 4 <= end)
			{
				ushort flags = SfntBinaryUtils.ReadUInt16BE(fontData, pos);
				ushort glyphIndex = SfntBinaryUtils.ReadUInt16BE(fontData, pos + 2);
				pos += 4;
				yield return glyphIndex;

				pos += (flags & CompositeArgsAreWords) != 0 ? 4 : 2;
				if ((flags & CompositeWeHaveATwoByTwo) != 0)
				{
					pos += 8;
				}
				else if ((flags & CompositeWeHaveAnXAndYScale) != 0)
				{
					pos += 4;
				}
				else if ((flags & CompositeWeHaveAScale) != 0)
				{
					pos += 2;
				}

				if ((flags & CompositeMoreComponents) == 0)
				{
					break;
				}
			}
		}

		private static int[] ReadLoca(byte[] fontData, int offset, ushort numGlyphs, short format)
		{
			int count = numGlyphs + 1;
			int[] result = new int[count];
			if (format == 0)
			{
				if (offset + count * 2 > fontData.Length)
				{
					return null;
				}
				for (int i = 0; i < count; i++)
				{
					result[i] = SfntBinaryUtils.ReadUInt16BE(fontData, offset + i * 2) * 2;
				}
			}
			else
			{
				if (offset + count * 4 > fontData.Length)
				{
					return null;
				}
				for (int i = 0; i < count; i++)
				{
					result[i] = (int)SfntBinaryUtils.ReadUInt32BE(fontData, offset + i * 4);
				}
			}
			return result;
		}

		private static byte[] WriteLoca(int[] loca, short format)
		{
			if (format == 0)
			{
				byte[] buffer = new byte[loca.Length * 2];
				for (int i = 0; i < loca.Length; i++)
				{
					SfntBinaryUtils.WriteUInt16BE(buffer, i * 2, (ushort)(loca[i] / 2));
				}
				return buffer;
			}
			byte[] buffer2 = new byte[loca.Length * 4];
			for (int i = 0; i < loca.Length; i++)
			{
				SfntBinaryUtils.WriteUInt32BE(buffer2, i * 4, (uint)loca[i]);
			}
			return buffer2;
		}
	}
}
