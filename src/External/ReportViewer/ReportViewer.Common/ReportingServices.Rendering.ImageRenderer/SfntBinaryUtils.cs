using System;
using System.Collections.Generic;

namespace ReportingServices.Rendering.ImageRenderer
{
	/// <summary>Which glyph-outline format an sfnt-wrapped font program uses - determines which glyph subsetter (<see cref="TrueTypeGlyphSubsetter"/>/<see cref="CffGlyphSubsetter"/>) applies, and which PDF FontFile/CIDFontType pair <see cref="PDFWriter"/> must declare for it.</summary>
	internal enum SfntOutlineFormat
	{
		TrueType,
		Cff,
		Unsupported
	}

	internal readonly struct SfntTableEntry
	{
		internal readonly int Offset;

		internal readonly int Length;

		internal SfntTableEntry(int offset, int length)
		{
			Offset = offset;
			Length = length;
		}
	}

	/// <summary>
	/// Low-level sfnt binary helpers shared by <see cref="TrueTypeGlyphSubsetter"/> and
	/// <see cref="CffGlyphSubsetter"/>: reading the top-level sfnt table directory,
	/// detecting whether the font's glyph outlines are TrueType (glyf/loca) or CFF, and
	/// reassembling a full sfnt binary from the original table directory plus a set of
	/// replacement table bytes (recomputing every table checksum and head's
	/// checkSumAdjustment per the OpenType spec, same as both subsetters need).
	/// </summary>
	internal static class SfntBinaryUtils
	{
		private const uint TagHead = 0x68656164u;

		private const uint TagOtto = 0x4F54544Fu;

		private const uint TagTtcf = 0x74746366u;

		private const uint TagTrueTypeVersion1 = 0x00010000u;

		private const uint TagMacTrue = 0x74727565u;

		internal static SfntOutlineFormat DetectOutlineFormat(byte[] fontData)
		{
			if (fontData == null || fontData.Length < 4)
			{
				return SfntOutlineFormat.Unsupported;
			}
			uint version = ReadUInt32BE(fontData, 0);
			if (version == TagOtto)
			{
				return SfntOutlineFormat.Cff;
			}
			if (version == TagTrueTypeVersion1 || version == TagMacTrue)
			{
				return SfntOutlineFormat.TrueType;
			}
			return SfntOutlineFormat.Unsupported; // includes 'ttcf' (TrueType Collection) - see TryExtractTtcFace/IsTtc: callers extract a single face into a standalone sfnt (detectable by this method) before reaching here
		}

		/// <summary>Whether <paramref name="fontData"/> is a TrueType Collection ('ttcf') container - a multi-face file, not itself a single sfnt program.</summary>
		internal static bool IsTtc(byte[] fontData)
		{
			return fontData != null && fontData.Length >= 4 && ReadUInt32BE(fontData, 0) == TagTtcf;
		}

		/// <summary>
		/// Extracts one face out of a TrueType Collection container as a standalone sfnt font
		/// program (table directory at offset 0, same shape <see cref="DetectOutlineFormat"/>/
		/// <see cref="TryReadTableDirectory"/>/<see cref="TrueTypeGlyphSubsetter"/>/
		/// <see cref="CffGlyphSubsetter"/> all assume). A TTC face's own table directory
		/// (found via the header's per-face offset table) already uses the exact same
		/// 12-byte-header-plus-table-records shape as a standalone sfnt - faces can share
		/// table data (e.g. 'glyf'/'hmtx' across weights of one family), but each table
		/// record's offset is always absolute into the whole container, so copying by those
		/// offsets (via <see cref="RebuildFontFromDirectory"/>) works whether or not the
		/// table happens to be shared with another face.
		/// </summary>
		internal static bool TryExtractTtcFace(byte[] ttcData, int faceIndex, out byte[] extractedFont)
		{
			extractedFont = null;
			if (!IsTtc(ttcData) || faceIndex < 0 || ttcData.Length < 12)
			{
				return false;
			}
			uint numFonts = ReadUInt32BE(ttcData, 8);
			if (faceIndex >= numFonts)
			{
				return false;
			}
			int offsetTableEntry = 12 + faceIndex * 4;
			if (offsetTableEntry + 4 > ttcData.Length)
			{
				return false;
			}
			int faceDirectoryOffset = (int)ReadUInt32BE(ttcData, offsetTableEntry);
			if (faceDirectoryOffset < 0 || faceDirectoryOffset + 12 > ttcData.Length)
			{
				return false;
			}
			ushort numTables = ReadUInt16BE(ttcData, faceDirectoryOffset + 4);
			if (faceDirectoryOffset + 12 + numTables * 16 > ttcData.Length)
			{
				return false;
			}
			try
			{
				extractedFont = RebuildFontFromDirectory(ttcData, faceDirectoryOffset, numTables, EmptyReplacements);
				return true;
			}
			catch (Exception)
			{
				extractedFont = null;
				return false;
			}
		}

		private static readonly Dictionary<uint, byte[]> EmptyReplacements = new Dictionary<uint, byte[]>();

		internal static bool TryReadTableDirectory(byte[] fontData, out ushort numTables, out Dictionary<uint, SfntTableEntry> tables)
		{
			numTables = 0;
			tables = null;
			if (fontData == null || fontData.Length < 12)
			{
				return false;
			}
			numTables = ReadUInt16BE(fontData, 4);
			var directory = new Dictionary<uint, SfntTableEntry>();
			for (int i = 0; i < numTables; i++)
			{
				int entryOffset = 12 + i * 16;
				if (entryOffset + 16 > fontData.Length)
				{
					return false;
				}
				uint tag = ReadUInt32BE(fontData, entryOffset);
				int tableOffset = (int)ReadUInt32BE(fontData, entryOffset + 8);
				int tableLength = (int)ReadUInt32BE(fontData, entryOffset + 12);
				directory[tag] = new SfntTableEntry(tableOffset, tableLength);
			}
			tables = directory;
			return true;
		}

		/// <summary>
		/// Reassembles a full sfnt binary from the original file's offset table (unchanged -
		/// table count/order never changes) plus each table's bytes, in the original
		/// directory's table order, substituting <paramref name="replacements"/> for the
		/// tables named in it and recomputing every table checksum plus head's whole-file
		/// checkSumAdjustment per the OpenType spec.
		/// </summary>
		internal static byte[] RebuildFont(byte[] original, ushort numTables, Dictionary<uint, byte[]> replacements)
		{
			return RebuildFontFromDirectory(original, 0, numTables, replacements);
		}

		/// <summary>
		/// Same as <see cref="RebuildFont"/>, but reads the source table directory starting at
		/// <paramref name="directoryOffset"/> instead of assuming it's at offset 0 - the shape
		/// <see cref="TryExtractTtcFace"/> needs to pull one face's directory out of a larger
		/// TrueType Collection container. Table record offsets/lengths are always absolute into
		/// <paramref name="original"/> regardless of <paramref name="directoryOffset"/>, per spec.
		/// </summary>
		internal static byte[] RebuildFontFromDirectory(byte[] original, int directoryOffset, ushort numTables, Dictionary<uint, byte[]> replacements)
		{
			var tags = new uint[numTables];
			var tableBytes = new byte[numTables][];
			for (int i = 0; i < numTables; i++)
			{
				int entryOffset = directoryOffset + 12 + i * 16;
				uint tag = ReadUInt32BE(original, entryOffset);
				int tableOffset = (int)ReadUInt32BE(original, entryOffset + 8);
				int tableLength = (int)ReadUInt32BE(original, entryOffset + 12);
				tags[i] = tag;
				tableBytes[i] = replacements.TryGetValue(tag, out byte[] replacement) ? replacement : CopyRange(original, tableOffset, tableLength);
			}

			int headIndex = Array.IndexOf(tags, TagHead);
			if (headIndex >= 0)
			{
				byte[] headCopy = (byte[])tableBytes[headIndex].Clone();
				WriteUInt32BE(headCopy, 8, 0u); // zero checkSumAdjustment before computing checksums
				tableBytes[headIndex] = headCopy;
			}

			int directorySize = 12 + numTables * 16;
			int runningOffset = directorySize;
			var tableOffsets = new int[numTables];
			var tableLengths = new int[numTables];
			var tableChecksums = new uint[numTables];
			var paddedTableBytes = new byte[numTables][];
			for (int i = 0; i < numTables; i++)
			{
				byte[] data = tableBytes[i];
				tableOffsets[i] = runningOffset;
				tableLengths[i] = data.Length;
				tableChecksums[i] = CalculateChecksum(data, 0, data.Length);
				int padding = (4 - data.Length % 4) % 4;
				paddedTableBytes[i] = padding == 0 ? data : Pad(data, padding);
				runningOffset += paddedTableBytes[i].Length;
			}

			byte[] output = new byte[runningOffset];
			Array.Copy(original, directoryOffset, output, 0, 12); // version/numTables/searchRange/entrySelector/rangeShift unchanged
			for (int i = 0; i < numTables; i++)
			{
				int entryOffset = 12 + i * 16;
				WriteUInt32BE(output, entryOffset, tags[i]);
				WriteUInt32BE(output, entryOffset + 4, tableChecksums[i]);
				WriteUInt32BE(output, entryOffset + 8, (uint)tableOffsets[i]);
				WriteUInt32BE(output, entryOffset + 12, (uint)tableLengths[i]);
				Array.Copy(paddedTableBytes[i], 0, output, tableOffsets[i], paddedTableBytes[i].Length);
			}

			uint fileChecksum = CalculateChecksum(output, 0, output.Length);
			uint checkSumAdjustment = unchecked(0xB1B0AFBAu - fileChecksum);
			if (headIndex >= 0)
			{
				WriteUInt32BE(output, tableOffsets[headIndex] + 8, checkSumAdjustment);
			}

			return output;
		}

		internal static uint CalculateChecksum(byte[] data, int offset, int length)
		{
			uint sum = 0;
			int end = offset + length;
			for (int i = offset; i < end; i += 4)
			{
				uint word = 0;
				for (int b = 0; b < 4; b++)
				{
					word <<= 8;
					if (i + b < end)
					{
						word |= data[i + b];
					}
				}
				sum = unchecked(sum + word);
			}
			return sum;
		}

		internal static byte[] CopyRange(byte[] source, int offset, int length)
		{
			byte[] result = new byte[length];
			Array.Copy(source, offset, result, 0, length);
			return result;
		}

		internal static byte[] Pad(byte[] data, int padding)
		{
			byte[] result = new byte[data.Length + padding];
			Array.Copy(data, result, data.Length);
			return result;
		}

		internal static ushort ReadUInt16BE(byte[] buffer, int offset)
		{
			return (ushort)((buffer[offset] << 8) | buffer[offset + 1]);
		}

		internal static uint ReadUInt32BE(byte[] buffer, int offset)
		{
			return ((uint)buffer[offset] << 24) | ((uint)buffer[offset + 1] << 16) | ((uint)buffer[offset + 2] << 8) | buffer[offset + 3];
		}

		internal static void WriteUInt16BE(byte[] buffer, int offset, ushort value)
		{
			buffer[offset] = (byte)(value >> 8);
			buffer[offset + 1] = (byte)value;
		}

		internal static void WriteUInt32BE(byte[] buffer, int offset, uint value)
		{
			buffer[offset] = (byte)(value >> 24);
			buffer[offset + 1] = (byte)(value >> 16);
			buffer[offset + 2] = (byte)(value >> 8);
			buffer[offset + 3] = (byte)value;
		}
	}
}
