using System;
using System.Collections.Generic;

namespace ReportingServices.Rendering.ImageRenderer
{
	/// <summary>
	/// The CFF-flavored ('OTTO') counterpart to <see cref="TrueTypeGlyphSubsetter"/>: reduces
	/// an embedded CFF font's CharStrings INDEX to only the glyphs actually used in the
	/// document, without renumbering glyph ids - same "zero the unused, never renumber"
	/// technique, so glyph-indexed Tj hex strings/CIDToGIDMap /Identity/hmtx-derived /W
	/// widths already emitted elsewhere need no change.
	///
	/// A CFF CharStrings INDEX has the same shape as TrueType's loca+glyf (an offset array
	/// plus a data blob, one entry per glyph id) so the same zero-unused-entries technique
	/// applies - but unlike glyf/loca, CFF's CharStrings INDEX is not the last thing in the
	/// file: the Top DICT (parsed here just far enough to find CharStrings/charset/
	/// Encoding/Private) stores absolute byte offsets to several other structures, some of
	/// which (typically Private DICT + its Local Subrs) are laid out *after* CharStrings.
	/// Shrinking CharStrings therefore requires patching those later offsets by the same
	/// delta - done in place, reusing each operand's original DICT integer encoding width
	/// (2/3/5-byte forms), so nothing else in the Top DICT INDEX (or anything before it)
	/// needs to move.
	///
	/// CID-keyed CFF (ROS/FDArray/FDSelect operators present) is also supported (2026-07-27,
	/// next day): unlike non-CID CFF, a CID-keyed font has no single top-level Private
	/// DICT - each Font DICT inside the FDArray INDEX has its own, and FDSelect maps each
	/// glyph id to which Font DICT (and therefore which Private DICT/Local Subrs) governs
	/// it. Since glyph ids are never renumbered, FDSelect's own contents never need to
	/// change - only its and FDArray's absolute position (if either moved past the shrunk
	/// CharStrings) needs the same delta-patch already used for charset/Encoding/Private
	/// above; each Font DICT inside FDArray is itself parsed with the same DICT parser used
	/// for the Top DICT (it only ever contains a Private operator, everything else this
	/// class reads being Top-DICT-only) so its own Private offset gets the identical patch.
	/// See <see cref="PatchCidKeyedStructures"/>.
	///
	/// Honest scope, all documented rather than silently dropped:
	/// - Only a single-font Top DICT INDEX (count == 1) is supported - a bare CFF
	///   "FontSet" with multiple Top DICTs never occurs inside an 'OTTO'-wrapped OpenType
	///   file in practice, but if seen this bails rather than guess which entry applies.
	/// - Charset/Encoding/Private offset operands are only patched when their original DICT
	///   encoding is the 3-byte (16-bit) or 5-byte (32-bit) integer form - the short 1-2
	///   byte forms only encode small values, which a real file-offset operand is never seen
	///   to use in practice; if one is, this bails rather than risk misencoding it.
	/// - No seac-style composite-glyph dependency resolution: Type2 charstrings have a
	///   deprecated `endchar`-with-4-args accented-character mechanism that references two
	///   other glyphs by StandardEncoding code rather than by direct glyph id (unlike
	///   TrueType's glyf composite records, which name component glyph ids directly). Modern
	///   font tooling (FontForge/AFDKO/Adobe) does not emit it - accented glyphs are
	///   precomposed outlines - so this is not tracked; if a real font relies on it, an
	///   accented glyph kept only via this deprecated mechanism could lose its dependency
	///   glyphs. Same class of honestly-scoped gap as this project's other approximations.
	/// - TrueType Collection ('ttcf') fonts remain out of scope for both subsetters (would
	///   need member-font selection/remapping) - <see cref="SfntBinaryUtils.DetectOutlineFormat"/>
	///   returns <see cref="SfntOutlineFormat.Unsupported"/> for them, so neither subsetter
	///   fires and the whole file is embedded unchanged, same as before this class existed.
	/// </summary>
	internal static class CffGlyphSubsetter
	{
		private const uint TagCff = 0x43464620u; // 'CFF '

		private const int OperatorRos = 1200 + 30;

		private const int OperatorFdArray = 1200 + 36;

		private const int OperatorFdSelect = 1200 + 37;

		private const int OperatorCharset = 15;

		private const int OperatorEncoding = 16;

		private const int OperatorCharStrings = 17;

		private const int OperatorPrivate = 18;

		private sealed class TopDictInfo
		{
			internal long CharStringsOffset = -1;

			internal long? CharsetOffset;

			internal int CharsetOperandStart;

			internal int CharsetOperandLen;

			internal long? EncodingOffset;

			internal int EncodingOperandStart;

			internal int EncodingOperandLen;

			internal long? PrivateOffset;

			internal int PrivateOffsetOperandStart;

			internal int PrivateOffsetOperandLen;

			internal bool IsCidKeyed;

			internal long? FdArrayOffset;

			internal int FdArrayOperandStart;

			internal int FdArrayOperandLen;

			internal long? FdSelectOffset;

			internal int FdSelectOperandStart;

			internal int FdSelectOperandLen;
		}

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
			if (SfntBinaryUtils.DetectOutlineFormat(fontData) != SfntOutlineFormat.Cff)
			{
				return false;
			}
			if (!SfntBinaryUtils.TryReadTableDirectory(fontData, out ushort numTables, out Dictionary<uint, SfntTableEntry> directory))
			{
				return false;
			}
			if (!directory.TryGetValue(TagCff, out SfntTableEntry cffEntry))
			{
				return false;
			}

			int cffStart = cffEntry.Offset;
			int cffLength = cffEntry.Length;
			if (cffLength < 4 || cffStart < 0 || cffStart + cffLength > fontData.Length)
			{
				return false;
			}

			int fileEnd = cffStart + cffLength;
			byte hdrSize = fontData[cffStart + 2];

			if (!TryReadIndex(fontData, cffStart + hdrSize, fileEnd, out int nameIndexTotalLength, out _, out _, out _))
			{
				return false;
			}
			int topDictIndexStart = cffStart + hdrSize + nameIndexTotalLength;
			if (!TryReadIndexEntries(fontData, topDictIndexStart, fileEnd, out List<(int Start, int Len)> topDictEntries))
			{
				return false;
			}
			if (topDictEntries.Count != 1)
			{
				return false; // FontSet with != 1 Top DICT - not a plain single-font CFF, out of scope
			}

			TopDictInfo topDict = ParseTopDict(fontData, topDictEntries[0].Start, topDictEntries[0].Len);
			if (topDict == null || topDict.CharStringsOffset < 0)
			{
				return false;
			}
			if (topDict.IsCidKeyed && !topDict.FdArrayOffset.HasValue)
			{
				return false; // malformed CID-keyed CFF - FDArray is mandatory whenever ROS/FDSelect is present
			}

			long charStringsAbsOffset = cffStart + topDict.CharStringsOffset;
			if (charStringsAbsOffset < 0 || charStringsAbsOffset >= fileEnd)
			{
				return false;
			}

			if (!TryReadIndex(fontData, (int)charStringsAbsOffset, fileEnd, out int charStringsTotalLength, out int numGlyphs, out byte offSize, out int[] csOffsets) || numGlyphs == 0)
			{
				return false;
			}
			long charStringsEndAbs = charStringsAbsOffset + charStringsTotalLength;

			var keep = new HashSet<ushort> { 0 }; // .notdef is always required; no seac dependency resolution - see class doc comment
			foreach (ushort glyphId in usedGlyphIds)
			{
				if (glyphId < numGlyphs)
				{
					keep.Add(glyphId);
				}
			}

			byte[] newCharStringsIndex = BuildCharStringsIndex(fontData, (int)charStringsAbsOffset, csOffsets, numGlyphs, offSize, keep);
			int delta = newCharStringsIndex.Length - charStringsTotalLength;

			byte[] cffBytes = SfntBinaryUtils.CopyRange(fontData, cffStart, cffLength);

			if (topDict.PrivateOffset.HasValue)
			{
				long absPrivate = cffStart + topDict.PrivateOffset.Value;
				if (absPrivate >= charStringsEndAbs)
				{
					if (!TryPatchOperand(cffBytes, topDict.PrivateOffsetOperandStart - cffStart, topDict.PrivateOffsetOperandLen, topDict.PrivateOffset.Value + delta))
					{
						return false;
					}
				}
			}
			if (topDict.CharsetOffset.HasValue && topDict.CharsetOffset.Value > 2) // 0/1/2 are predefined-charset ids, not offsets
			{
				long absCharset = cffStart + topDict.CharsetOffset.Value;
				if (absCharset >= charStringsEndAbs && !TryPatchOperand(cffBytes, topDict.CharsetOperandStart - cffStart, topDict.CharsetOperandLen, topDict.CharsetOffset.Value + delta))
				{
					return false;
				}
			}
			if (topDict.EncodingOffset.HasValue && topDict.EncodingOffset.Value > 1) // 0/1 are predefined-encoding ids, not offsets
			{
				long absEncoding = cffStart + topDict.EncodingOffset.Value;
				if (absEncoding >= charStringsEndAbs && !TryPatchOperand(cffBytes, topDict.EncodingOperandStart - cffStart, topDict.EncodingOperandLen, topDict.EncodingOffset.Value + delta))
				{
					return false;
				}
			}
			if (topDict.IsCidKeyed && !PatchCidKeyedStructures(fontData, cffStart, fileEnd, charStringsEndAbs, delta, topDict, cffBytes))
			{
				return false;
			}

			int csStartRel = (int)(charStringsAbsOffset - cffStart);
			int csEndRel = (int)(charStringsEndAbs - cffStart);
			int tailLen = cffLength - csEndRel;
			byte[] newCffBytes = new byte[cffLength + delta];
			Array.Copy(cffBytes, 0, newCffBytes, 0, csStartRel);
			Array.Copy(newCharStringsIndex, 0, newCffBytes, csStartRel, newCharStringsIndex.Length);
			Array.Copy(cffBytes, csEndRel, newCffBytes, csStartRel + newCharStringsIndex.Length, tailLen);

			subsetted = SfntBinaryUtils.RebuildFont(fontData, numTables, new Dictionary<uint, byte[]>
			{
				[TagCff] = newCffBytes
			});
			return true;
		}

		/// <summary>
		/// Patches every CID-keyed-specific offset that could be affected by the CharStrings
		/// shrink: the Top DICT's own FDArray/FDSelect offsets, plus each Font DICT inside
		/// FDArray's own Private DICT offset (a CID-keyed CFF has no single top-level Private
		/// DICT - each Font DICT in the FDArray has its own). FDSelect's own internal
		/// contents (format 0 glyph-to-FD array, or format 3 ranges) never need to change,
		/// since glyph ids are never renumbered - only the block's absolute position, if it
		/// moved past the shrunk CharStrings, needs patching.
		/// </summary>
		private static bool PatchCidKeyedStructures(byte[] fontData, int cffStart, int fileEnd, long charStringsEndAbs, int delta, TopDictInfo topDict, byte[] cffBytes)
		{
			long absFdArray = cffStart + topDict.FdArrayOffset.Value;
			if (absFdArray >= charStringsEndAbs && !TryPatchOperand(cffBytes, topDict.FdArrayOperandStart - cffStart, topDict.FdArrayOperandLen, topDict.FdArrayOffset.Value + delta))
			{
				return false;
			}

			if (!TryReadIndexEntries(fontData, (int)absFdArray, fileEnd, out List<(int Start, int Len)> fontDictEntries))
			{
				return false;
			}
			foreach ((int Start, int Len) fontDict in fontDictEntries)
			{
				TopDictInfo fdInfo = ParseTopDict(fontData, fontDict.Start, fontDict.Len);
				if (fdInfo == null)
				{
					return false;
				}
				if (fdInfo.PrivateOffset.HasValue)
				{
					long absFdPrivate = cffStart + fdInfo.PrivateOffset.Value;
					if (absFdPrivate >= charStringsEndAbs && !TryPatchOperand(cffBytes, fdInfo.PrivateOffsetOperandStart - cffStart, fdInfo.PrivateOffsetOperandLen, fdInfo.PrivateOffset.Value + delta))
					{
						return false;
					}
				}
			}

			if (topDict.FdSelectOffset.HasValue)
			{
				long absFdSelect = cffStart + topDict.FdSelectOffset.Value;
				if (absFdSelect >= charStringsEndAbs && !TryPatchOperand(cffBytes, topDict.FdSelectOperandStart - cffStart, topDict.FdSelectOperandLen, topDict.FdSelectOffset.Value + delta))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>Reads a CFF INDEX structure's header (count/offSize/offsets) at <paramref name="start"/> and returns its total byte length (header + offset array + data).</summary>
		private static bool TryReadIndex(byte[] data, int start, int fileEnd, out int totalLength, out int count, out byte offSize, out int[] offsets)
		{
			totalLength = 0;
			count = 0;
			offSize = 0;
			offsets = null;
			if (start < 0 || start + 2 > fileEnd || start + 2 > data.Length)
			{
				return false;
			}
			ushort entryCount = SfntBinaryUtils.ReadUInt16BE(data, start);
			if (entryCount == 0)
			{
				totalLength = 2;
				count = 0;
				offsets = Array.Empty<int>();
				return true;
			}
			if (start + 3 > fileEnd)
			{
				return false;
			}
			byte os = data[start + 2];
			if (os < 1 || os > 4)
			{
				return false;
			}
			int offsetArrayStart = start + 3;
			int offsetArrayLen = (entryCount + 1) * os;
			if (offsetArrayStart + offsetArrayLen > fileEnd || offsetArrayStart + offsetArrayLen > data.Length)
			{
				return false;
			}
			int[] parsedOffsets = new int[entryCount + 1];
			for (int i = 0; i <= entryCount; i++)
			{
				int value = 0;
				int p = offsetArrayStart + i * os;
				for (int k = 0; k < os; k++)
				{
					value = (value << 8) | data[p + k];
				}
				parsedOffsets[i] = value;
			}
			int dataStart = offsetArrayStart + offsetArrayLen;
			int dataLen = parsedOffsets[entryCount] - 1;
			if (dataLen < 0 || dataStart + dataLen > fileEnd || dataStart + dataLen > data.Length)
			{
				return false;
			}
			totalLength = 3 + offsetArrayLen + dataLen;
			count = entryCount;
			offSize = os;
			offsets = parsedOffsets;
			return true;
		}

		private static bool TryReadIndexEntries(byte[] data, int start, int fileEnd, out List<(int Start, int Len)> entries)
		{
			entries = null;
			if (!TryReadIndex(data, start, fileEnd, out int totalLength, out int count, out byte offSize, out int[] offsets))
			{
				return false;
			}
			var result = new List<(int, int)>(count);
			int dataStart = start + 3 + (count + 1) * offSize;
			for (int i = 0; i < count; i++)
			{
				int entryStart = dataStart + offsets[i] - 1;
				int entryLen = offsets[i + 1] - offsets[i];
				result.Add((entryStart, entryLen));
			}
			entries = result;
			return true;
		}

		/// <summary>
		/// Parses a Top DICT's operator/operand stream far enough to capture the four
		/// offset-bearing operators this class cares about (charset/Encoding/CharStrings/
		/// Private), plus whether ROS (CID-keyed marker) is present - preserving each
		/// captured operand's original byte position/length so it can be patched in place
		/// later without needing to re-serialize the whole DICT.
		/// </summary>
		private static TopDictInfo ParseTopDict(byte[] data, int dictStart, int dictLength)
		{
			var info = new TopDictInfo();
			var operands = new List<(long Value, int Start, int Len)>();
			int pos = dictStart;
			int end = dictStart + dictLength;
			while (pos < end)
			{
				int b0 = data[pos];
				if (b0 <= 21)
				{
					int operatorCode;
					if (b0 == 12)
					{
						if (pos + 1 >= end)
						{
							return null;
						}
						operatorCode = 1200 + data[pos + 1];
						pos += 2;
					}
					else
					{
						operatorCode = b0;
						pos += 1;
					}
					ApplyOperator(operatorCode, operands, info);
					operands.Clear();
				}
				else if (b0 == 28)
				{
					if (pos + 3 > end)
					{
						return null;
					}
					short value = (short)((data[pos + 1] << 8) | data[pos + 2]);
					operands.Add((value, pos, 3));
					pos += 3;
				}
				else if (b0 == 29)
				{
					if (pos + 5 > end)
					{
						return null;
					}
					int value = (int)(((uint)data[pos + 1] << 24) | ((uint)data[pos + 2] << 16) | ((uint)data[pos + 3] << 8) | data[pos + 4]);
					operands.Add((value, pos, 5));
					pos += 5;
				}
				else if (b0 == 30)
				{
					int start = pos;
					pos += 1;
					bool done = false;
					while (pos < end && !done)
					{
						byte nibbleByte = data[pos];
						pos += 1;
						int hi = nibbleByte >> 4;
						int lo = nibbleByte & 0xF;
						if (hi == 0xF || lo == 0xF)
						{
							done = true;
						}
					}
					if (!done)
					{
						return null;
					}
					operands.Add((0, start, pos - start)); // real-number value not needed by any operator this class reads
				}
				else if (b0 >= 32 && b0 <= 246)
				{
					operands.Add((b0 - 139, pos, 1));
					pos += 1;
				}
				else if (b0 >= 247 && b0 <= 250)
				{
					if (pos + 2 > end)
					{
						return null;
					}
					operands.Add(((b0 - 247) * 256 + data[pos + 1] + 108, pos, 2));
					pos += 2;
				}
				else if (b0 >= 251 && b0 <= 254)
				{
					if (pos + 2 > end)
					{
						return null;
					}
					operands.Add((-(b0 - 251) * 256 - data[pos + 1] - 108, pos, 2));
					pos += 2;
				}
				else
				{
					return null; // reserved byte value (22-27, 31, 255) - malformed DICT
				}
			}
			return info;
		}

		private static void ApplyOperator(int operatorCode, List<(long Value, int Start, int Len)> operands, TopDictInfo info)
		{
			switch (operatorCode)
			{
				case OperatorCharset:
					if (operands.Count >= 1)
					{
						var last = operands[operands.Count - 1];
						info.CharsetOffset = last.Value;
						info.CharsetOperandStart = last.Start;
						info.CharsetOperandLen = last.Len;
					}
					break;
				case OperatorEncoding:
					if (operands.Count >= 1)
					{
						var last = operands[operands.Count - 1];
						info.EncodingOffset = last.Value;
						info.EncodingOperandStart = last.Start;
						info.EncodingOperandLen = last.Len;
					}
					break;
				case OperatorCharStrings:
					if (operands.Count >= 1)
					{
						info.CharStringsOffset = operands[operands.Count - 1].Value;
					}
					break;
				case OperatorPrivate:
					if (operands.Count >= 2)
					{
						var offsetOperand = operands[operands.Count - 1];
						info.PrivateOffset = offsetOperand.Value;
						info.PrivateOffsetOperandStart = offsetOperand.Start;
						info.PrivateOffsetOperandLen = offsetOperand.Len;
					}
					break;
				case OperatorRos:
					info.IsCidKeyed = true;
					break;
				case OperatorFdArray:
					info.IsCidKeyed = true; // FDArray/FDSelect only ever accompany CID-keyed CFF; treat the same as ROS
					if (operands.Count >= 1)
					{
						var last = operands[operands.Count - 1];
						info.FdArrayOffset = last.Value;
						info.FdArrayOperandStart = last.Start;
						info.FdArrayOperandLen = last.Len;
					}
					break;
				case OperatorFdSelect:
					info.IsCidKeyed = true;
					if (operands.Count >= 1)
					{
						var last = operands[operands.Count - 1];
						info.FdSelectOffset = last.Value;
						info.FdSelectOperandStart = last.Start;
						info.FdSelectOperandLen = last.Len;
					}
					break;
			}
		}

		/// <summary>Re-encodes <paramref name="newValue"/> in place at [<paramref name="start"/>, <paramref name="start"/>+<paramref name="len"/>) using the same DICT integer encoding form (3-byte/16-bit or 5-byte/32-bit) the original operand used - fails rather than guess for any other original encoding width.</summary>
		private static bool TryPatchOperand(byte[] data, int start, int len, long newValue)
		{
			if (len == 3)
			{
				if (newValue < short.MinValue || newValue > short.MaxValue)
				{
					return false;
				}
				data[start] = 28;
				short value = (short)newValue;
				data[start + 1] = (byte)(value >> 8);
				data[start + 2] = (byte)value;
				return true;
			}
			if (len == 5)
			{
				if (newValue < int.MinValue || newValue > int.MaxValue)
				{
					return false;
				}
				data[start] = 29;
				int value = (int)newValue;
				data[start + 1] = (byte)(value >> 24);
				data[start + 2] = (byte)(value >> 16);
				data[start + 3] = (byte)(value >> 8);
				data[start + 4] = (byte)value;
				return true;
			}
			return false; // short (1-2 byte) forms only encode small values - a real file offset is never seen using them in practice
		}

		private static byte[] BuildCharStringsIndex(byte[] fontData, int indexStart, int[] offsets, int numGlyphs, byte offSize, HashSet<ushort> keep)
		{
			int headerLen = 3; // count(2) + offSize(1)
			int offsetArrayLen = (numGlyphs + 1) * offSize;
			int dataStart = indexStart + headerLen + offsetArrayLen;

			var newData = new List<byte>();
			int[] newOffsets = new int[numGlyphs + 1];
			newOffsets[0] = 1;
			for (int glyphId = 0; glyphId < numGlyphs; glyphId++)
			{
				int start = offsets[glyphId] - 1;
				int endOffset = offsets[glyphId + 1] - 1;
				if (keep.Contains((ushort)glyphId) && endOffset > start)
				{
					for (int b = dataStart + start; b < dataStart + endOffset; b++)
					{
						newData.Add(fontData[b]);
					}
				}
				newOffsets[glyphId + 1] = newData.Count + 1;
			}
			byte[] newDataArray = newData.ToArray();

			byte[] result = new byte[headerLen + offsetArrayLen + newDataArray.Length];
			SfntBinaryUtils.WriteUInt16BE(result, 0, (ushort)numGlyphs);
			result[2] = offSize;
			for (int i = 0; i <= numGlyphs; i++)
			{
				WriteOffsetBE(result, 3 + i * offSize, newOffsets[i], offSize);
			}
			Array.Copy(newDataArray, 0, result, headerLen + offsetArrayLen, newDataArray.Length);
			return result;
		}

		private static void WriteOffsetBE(byte[] buffer, int position, int value, int width)
		{
			for (int i = 0; i < width; i++)
			{
				buffer[position + i] = (byte)(value >> (8 * (width - 1 - i)));
			}
		}
	}
}
