using OpenMcdf;
using System;
using System.IO;
using System.Text;

namespace ReportingServices.Rendering.WordRenderer
{
	// Portable OLE Compound File Binary Format (CFBF) writer for the WORD (binary Word 97)
	// renderer, replacing a Windows-only implementation that called ole32.dll's
	// StgCreateDocfile/IStorage/IStream COM interop directly (impossible on Linux - see
	// tasks/word-renderer-cross-platform.md). Built on the OpenMcdf NuGet package, which
	// implements the CFBF spec in managed code with no native/COM dependency.
	internal static class StructuredStorage
	{
		// SummaryInformation FMTID (MS-OLEPS 2.19) - the standard property set that carries
		// Title/Author/Comments in a compound file's file-properties metadata.
		private static readonly Guid SummaryInformationFmtid = new Guid("F29F85E0-4FF9-1068-AB91-08002B27B3D9");

		// Per MS-OLE's naming convention, property-set streams are named with a leading
		// U+0005 control character so they don't collide with user-visible stream names.
		private const string SummaryInformationStreamName = "SummaryInformation";

		private const int PIDSI_CODEPAGE = 1;

		private const short VT_I2 = 2;

		private const int VT_LPWSTR = 31;

		// CP_WINUNICODE - the codepage value MS-OLEPS requires whenever string properties are
		// stored as VT_LPWSTR (UTF-16) rather than VT_LPSTR (codepage-dependent ANSI).
		private const short CP_WINUNICODE = 1200;

		internal static bool CreateMultiStreamFile(Stream[] sources, string[] streamNames, string clsId, string author, string title, string comments, Stream output, bool forceInMemory)
		{
			using RootStorage rootStorage = RootStorage.Create(output, OpenMcdf.Version.V3, StorageModeFlags.LeaveOpen);
			for (int i = 0; i < streamNames.Length; i++)
			{
				sources[i].Seek(0L, SeekOrigin.Begin);
				using (CfbStream cfbStream = rootStorage.CreateStream(streamNames[i]))
				{
					sources[i].CopyTo(cfbStream);
				}
				sources[i] = null;
			}
			// Title/author/comments are supplementary file-properties metadata, not required for
			// the document itself to be valid - if anything here goes wrong, skip it rather than
			// fail the whole render (matches the original code's own "only write if non-empty" tolerance).
			try
			{
				WriteSummaryInformation(rootStorage, title, author, comments);
			}
			catch (Exception)
			{
			}
			rootStorage.Flush(consolidate: true);
			return true;
		}

		// Hand-written MS-OLEPS PropertySetStream, since OpenMcdf (a CFBF container library)
		// has no property-set support of its own - CFBF just sees this as an ordinary stream
		// of bytes. Format: MS-OLEPS 2.21 (PropertySetStream) with a single section (2.19) for
		// FMTID_SummaryInformation, containing the mandatory codepage property plus whichever
		// of Title/Author/Comments were actually supplied.
		private static void WriteSummaryInformation(RootStorage rootStorage, string title, string author, string comments)
		{
			(int propertyId, string value)[] properties = new (int, string)[3]
			{
				(2, title),
				(4, author),
				(6, comments)
			};
			int suppliedCount = 0;
			foreach (var (_, value) in properties)
			{
				if (!string.IsNullOrEmpty(value))
				{
					suppliedCount++;
				}
			}
			if (suppliedCount == 0)
			{
				return;
			}

			using MemoryStream buffer = new MemoryStream();
			using (BinaryWriter writer = new BinaryWriter(buffer, Encoding.Unicode, leaveOpen: true))
			{
				// PropertySetStream header (MS-OLEPS 2.21): byte order mark, version, OS identifier,
				// a zero CLSID (unused by this single-section case), and one property set.
				writer.Write((ushort)0xFFFE);
				writer.Write((ushort)0);
				writer.Write(0);
				writer.Write(new byte[16]);
				writer.Write(1);
				writer.Write(SummaryInformationFmtid.ToByteArray());
				long sectionOffsetFieldPosition = writer.BaseStream.Position;
				writer.Write(0); // Offset0 placeholder, patched below once the section's real start is known.
				long sectionStart = writer.BaseStream.Position;

				int propertyCount = suppliedCount + 1; // +1 for the mandatory codepage property.
				writer.Write(0); // cbSection placeholder, patched below once the section's total size is known.
				writer.Write(propertyCount);
				long idOffsetTableStart = writer.BaseStream.Position;
				for (int i = 0; i < propertyCount; i++)
				{
					writer.Write(0);
					writer.Write(0);
				}

				int[] propertyIds = new int[propertyCount];
				long[] valueOffsets = new long[propertyCount];
				int slot = 0;
				propertyIds[slot] = PIDSI_CODEPAGE;
				valueOffsets[slot] = writer.BaseStream.Position - sectionStart;
				writer.Write((int)VT_I2);
				writer.Write(CP_WINUNICODE);
				writer.Write((short)0);
				slot++;
				foreach (var (propertyId, value) in properties)
				{
					if (!string.IsNullOrEmpty(value))
					{
						propertyIds[slot] = propertyId;
						valueOffsets[slot] = writer.BaseStream.Position - sectionStart;
						WriteLpwstr(writer, value);
						slot++;
					}
				}
				long sectionEnd = writer.BaseStream.Position;

				writer.BaseStream.Position = idOffsetTableStart;
				for (int i = 0; i < propertyCount; i++)
				{
					writer.Write(propertyIds[i]);
					writer.Write((int)valueOffsets[i]);
				}
				writer.BaseStream.Position = sectionStart;
				writer.Write((int)(sectionEnd - sectionStart));
				writer.BaseStream.Position = sectionOffsetFieldPosition;
				writer.Write((int)sectionStart);
			}

			buffer.Position = 0L;
			using CfbStream cfbStream = rootStorage.CreateStream(SummaryInformationStreamName);
			buffer.CopyTo(cfbStream);
		}

		// VT_LPWSTR (MS-OLEPS 2.15): a 4-byte type tag, a 4-byte character count (including the
		// null terminator), the UTF-16LE characters, the null terminator, then zero-padding out
		// to the next 4-byte boundary.
		private static void WriteLpwstr(BinaryWriter writer, string value)
		{
			writer.Write(VT_LPWSTR);
			writer.Write(value.Length + 1);
			byte[] chars = Encoding.Unicode.GetBytes(value);
			writer.Write(chars);
			writer.Write((short)0);
			int writtenLength = chars.Length + 2;
			int padding = (4 - writtenLength % 4) % 4;
			for (int i = 0; i < padding; i++)
			{
				writer.Write((byte)0);
			}
		}
	}
}
