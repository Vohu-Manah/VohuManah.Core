using ReportingServices.Rendering.RPLProcessing;
using System.Drawing;

namespace ReportingServices.Rendering.RichText
{
	internal interface ITextBoxProps
	{
		RPLFormat.WritingModes WritingMode
		{
			get;
		}

		RPLFormat.TextAlignments DefaultAlignment
		{
			get;
		}

		RPLFormat.Directions Direction
		{
			get;
		}

		Color BackgroundColor
		{
			get;
		}

		bool CanGrow
		{
			get;
		}

		void DrawTextRun(TextRun run, Paragraph paragraph, Win32DCSafeHandle hdc, float dpiX, FontCache fontCache, int x, int y, int baselineY, int lineHeight, Rectangle layoutRectangle);

		void DrawClippedTextRun(TextRun run, Paragraph paragraph, Win32DCSafeHandle hdc, float dpiX, FontCache fontCache, int x, int y, int baselineY, int lineHeight, Rectangle layoutRectangle, uint fontColorOverride, Rectangle clipRect);

		// Cross-platform counterpart to RenderHighlightedTextRun's GDI+ g.FillRectangle
		// call (see TextBox.cs) - used when no real System.Drawing.Graphics exists to
		// fill through (non-Windows). rect is already fully resolved to device pixels
		// (layoutRectangle offset included), same convention as DrawTextRun's position.
		void FillHighlightRectangle(FontCache fontCache, Rectangle rect, Color color);
	}
}
