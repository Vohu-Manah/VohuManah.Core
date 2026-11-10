using SkiaSharp;

namespace ReportingServices.Rendering.ImageRenderer
{
	/// <summary>
	/// Cross-platform counterpart to <see cref="FontPackage.CheckEmbeddingRights"/> - reads
	/// the OS/2 table's <c>fsType</c> embedding-permission bits directly from an
	/// <see cref="SKTypeface"/> (<see cref="SKTypeface.GetTableData"/>), which needs no
	/// Win32 HFONT/HDC, unlike <see cref="FontPackage.CheckEmbeddingRights"/>'s
	/// <c>TTGetEmbeddingType</c> P/Invoke.
	/// </summary>
	internal static class SkiaFontEmbeddingRights
	{
		// Big-endian ASCII "OS/2" packed the same way SKTypeface.GetTableTags() reports
		// table tags (verified against a real Arial font: byte offsets 8-9 hold fsType).
		private const uint OS2TableTag = 0x4F532F32u;

		private const int FsTypeOffset = 8;

		// OpenType OS/2 fsType bit 1: "Restricted License embedding" - the only fsType bit
		// that forbids embedding outright (Preview & Print / Editable / no-subsetting /
		// bitmap-only are usage restrictions for a PDF viewer/print client, not embedding
		// prohibitions), matching FontPackage.CheckEmbeddingRights's EMBED_NOEMBEDDING check.
		private const ushort RestrictedLicenseEmbedding = 0x0002;

		/// <summary>
		/// Returns whether <paramref name="typeface"/> may be embedded. A missing/malformed
		/// OS/2 table (some non-TrueType/OpenType fonts have none) is treated as unrestricted,
		/// since there is nothing to indicate otherwise.
		/// </summary>
		internal static bool CanEmbed(SKTypeface typeface)
		{
			if (typeface == null)
			{
				return false;
			}

			byte[] os2Table = typeface.GetTableData(OS2TableTag);
			if (os2Table == null || os2Table.Length < FsTypeOffset + 2)
			{
				return true;
			}

			ushort fsType = (ushort)((os2Table[FsTypeOffset] << 8) | os2Table[FsTypeOffset + 1]);
			return CanEmbedFsType(fsType);
		}

		/// <summary>Pure bit-check, split out from <see cref="CanEmbed"/> so it's testable without a real font/typeface.</summary>
		internal static bool CanEmbedFsType(ushort fsType)
		{
			return (fsType & RestrictedLicenseEmbedding) == 0;
		}
	}
}
