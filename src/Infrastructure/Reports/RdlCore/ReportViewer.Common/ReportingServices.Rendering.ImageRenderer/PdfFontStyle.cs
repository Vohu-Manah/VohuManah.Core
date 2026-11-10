using System;
using System.Drawing;

namespace ReportingServices.Rendering.ImageRenderer
{
	/// <summary>
	/// Cross-platform stand-in for System.Drawing.FontStyle, used by PDFFont so the
	/// PDF font model doesn't carry a GDI+ type. Bit values are kept 1:1 with FontStyle
	/// so the conversion helpers are trivial casts.
	/// </summary>
	[Flags]
	internal enum PdfFontStyle
	{
		Regular = 0,
		Bold = 1,
		Italic = 2,
		Underline = 4,
		Strikeout = 8
	}

	internal static class PdfFontStyleConverter
	{
		internal static PdfFontStyle FromGdiFontStyle(FontStyle style)
		{
			return (PdfFontStyle)style;
		}

		internal static FontStyle ToGdiFontStyle(PdfFontStyle style)
		{
			return (FontStyle)style;
		}
	}
}
