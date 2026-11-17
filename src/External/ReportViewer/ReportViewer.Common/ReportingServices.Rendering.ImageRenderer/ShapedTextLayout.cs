using System;
using System.Collections.Generic;
using ReportingServices.Rendering.RichText;

namespace ReportingServices.Rendering.ImageRenderer
{
	/// <summary>
	/// Caches one <see cref="SkiaCachedFont"/> per distinct (family, size, bold, italic)
	/// key for the lifetime of a single render (owned by <see cref="PDFWriter"/>, disposed
	/// alongside it) - avoids reconstructing a SkiaCachedFont/SKShaper per word, matching
	/// the existing RichText.FontCache's per-render caching shape.
	/// </summary>
	internal sealed class ShapedFontCache : IDisposable
	{
		private readonly Dictionary<string, SkiaCachedFont> m_fonts = new Dictionary<string, SkiaCachedFont>();

		internal SkiaCachedFont GetFont(string fontFamily, float fontSizePoints, bool bold, bool italic)
		{
			string key = fontFamily + "|" + fontSizePoints.ToString(System.Globalization.CultureInfo.InvariantCulture) + "|" + (bold ? "b" : "n") + (italic ? "i" : "n");
			if (!m_fonts.TryGetValue(key, out SkiaCachedFont font))
			{
				font = new SkiaCachedFont(fontFamily, fontSizePoints, bold, italic);
				m_fonts.Add(key, font);
			}
			return font;
		}

		public void Dispose()
		{
			foreach (SkiaCachedFont font in m_fonts.Values)
			{
				font.Dispose();
			}
			m_fonts.Clear();
		}
	}

	/// <summary>
	/// Real per-character glyph-advance and soft-break-opportunity measurement for the
	/// cross-platform PDF text path, replacing <see cref="ApproximateTextMetrics"/>'s
	/// per-character-class table with actual shaped widths from
	/// <see cref="UnicodeParagraphShaper"/> (itemization + HarfBuzz shaping + line-break
	/// analysis - tasks/pdf-text-shaping-abstraction.md's P4 step 3 prototypes). Still
	/// only used to decide *where* to wrap and how wide to draw decoration rectangles -
	/// the actual glyphs are still drawn via PDFWriter's base-14-font Tj path, which
	/// relies on the PDF reader's own font metrics for on-page glyph positioning. Real
	/// font embedding (glyph-indexed Tj/TJ output) remains out of scope, per the standing
	/// decision to defer additional font families/embedding (docs/decisions.md).
	/// </summary>
	internal static class ShapedTextMetrics
	{
		internal static void Measure(string text, string fontFamily, float fontSizePoints, bool bold, bool italic, ShapedFontCache fontCache, out float[] charWidthsPoints, out bool[] canBreakBeforeChar)
		{
			if (string.IsNullOrEmpty(text))
			{
				charWidthsPoints = Array.Empty<float>();
				canBreakBeforeChar = Array.Empty<bool>();
				return;
			}

			charWidthsPoints = new float[text.Length];
			canBreakBeforeChar = new bool[text.Length];

			SkiaCachedFont font = fontCache.GetFont(fontFamily, fontSizePoints, bold, italic);
			List<ShapedRunItem> items = UnicodeParagraphShaper.Shape(text, font);
			foreach (ShapedRunItem item in items)
			{
				GlyphShapeData shape = item.GlyphData.GlyphScriptShapeData;
				int[] rawAdvances = item.GlyphData.RawAdvances;
				for (int c = 0; c < item.Length; c++)
				{
					int globalIndex = item.CharPos + c;
					canBreakBeforeChar[globalIndex] = item.ScriptLogAttr[c].IsSoftBreak;
					bool isFirstCharOfCluster = c == 0 || shape.Clusters[c] != shape.Clusters[c - 1];
					if (isFirstCharOfCluster)
					{
						int glyphIndex = shape.Clusters[c];
						if (glyphIndex >= 0 && glyphIndex < rawAdvances.Length)
						{
							charWidthsPoints[globalIndex] = rawAdvances[glyphIndex];
						}
					}
				}
			}
		}

		internal static float MeasureTotalWidthPoints(string text, string fontFamily, float fontSizePoints, bool bold, bool italic, ShapedFontCache fontCache)
		{
			Measure(text, fontFamily, fontSizePoints, bold, italic, fontCache, out float[] charWidths, out _);
			float total = 0f;
			foreach (float w in charWidths)
			{
				total += w;
			}
			return total;
		}
	}

	/// <summary>
	/// Greedy line-breaking for single-style text using real shaped per-character widths
	/// and soft-break opportunities (<see cref="ShapedTextMetrics"/>), replacing
	/// <see cref="SimpleTextWrapper"/>'s space-splitting/approximate-width approach.
	/// Breaks are only taken where <see cref="UnicodeLineBreakAnalyzer"/> allows one
	/// (after whitespace or a hyphen); a single unbreakable run of characters wider than
	/// the box is allowed to overflow rather than infinite-loop, same fallback as the
	/// approximate wrapper it replaces.
	/// </summary>
	internal static class ShapedTextWrapper
	{
		internal static List<string> Wrap(string text, string fontFamily, float fontSizePoints, bool bold, bool italic, float maxWidthPoints, ShapedFontCache fontCache)
		{
			var lines = new List<string>();
			if (string.IsNullOrEmpty(text))
			{
				lines.Add(string.Empty);
				return lines;
			}

			foreach (string paragraphLine in text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
			{
				WrapLine(paragraphLine, fontFamily, fontSizePoints, bold, italic, maxWidthPoints, fontCache, lines);
			}
			return lines;
		}

		private static void WrapLine(string line, string fontFamily, float fontSizePoints, bool bold, bool italic, float maxWidthPoints, ShapedFontCache fontCache, List<string> lines)
		{
			if (maxWidthPoints <= 0f || line.Length == 0)
			{
				lines.Add(line);
				return;
			}

			ShapedTextMetrics.Measure(line, fontFamily, fontSizePoints, bold, italic, fontCache, out float[] charWidths, out bool[] canBreakBefore);

			int segStart = 0;
			int lastBreak = -1;
			float widthSinceSegStart = 0f;

			for (int i = 0; i < line.Length; i++)
			{
				if (i > segStart && canBreakBefore[i])
				{
					lastBreak = i;
				}

				float charWidth = charWidths[i];
				bool overflow = i > segStart && widthSinceSegStart + charWidth > maxWidthPoints;
				if (overflow && lastBreak > segStart)
				{
					lines.Add(line.Substring(segStart, lastBreak - segStart));
					segStart = lastBreak;
					lastBreak = -1;
					widthSinceSegStart = 0f;
					for (int k = segStart; k <= i; k++)
					{
						widthSinceSegStart += charWidths[k];
					}
					continue;
				}

				widthSinceSegStart += charWidth;
			}

			lines.Add(line.Substring(segStart));
		}
	}

	/// <summary>
	/// Greedy line-breaking across multiple styled runs within a paragraph, using real
	/// shaped per-character widths and soft-break opportunities per run (each run is
	/// shaped with its own style's font). Replaces <see cref="StyledTextWrapper"/>'s
	/// space-splitting/approximate-width approach; produces the same
	/// <see cref="StyledLineFragment"/> shape so PDFWriter's drawing code needs no change.
	/// Same fallback as <see cref="ShapedTextWrapper"/> for an unbreakable run wider than
	/// the box: it's allowed to overflow rather than infinite-loop.
	/// </summary>
	internal static class ShapedStyledTextWrapper
	{
		internal static List<List<StyledLineFragment>> WrapParagraph(IReadOnlyList<(string Text, ITextRunProps Style)> runs, float maxWidthPoints, ShapedFontCache fontCache)
		{
			var lines = new List<List<StyledLineFragment>>();
			var currentLine = new List<StyledLineFragment>();
			float currentWidth = 0f;
			bool lineHasContent = false;

			void FlushLine()
			{
				lines.Add(currentLine);
				currentLine = new List<StyledLineFragment>();
				currentWidth = 0f;
				lineHasContent = false;
			}

			void AppendFragment(string text, ITextRunProps style)
			{
				if (text.Length == 0)
				{
					return;
				}
				if (currentLine.Count > 0 && ReferenceEquals(currentLine[currentLine.Count - 1].Style, style))
				{
					StyledLineFragment last = currentLine[currentLine.Count - 1];
					currentLine[currentLine.Count - 1] = new StyledLineFragment(last.Text + text, style);
				}
				else
				{
					currentLine.Add(new StyledLineFragment(text, style));
				}
				lineHasContent = true;
			}

			foreach ((string runText, ITextRunProps style) in runs)
			{
				string[] pieces = runText.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
				for (int pieceIndex = 0; pieceIndex < pieces.Length; pieceIndex++)
				{
					string piece = pieces[pieceIndex];
					if (piece.Length > 0)
					{
						ShapedTextMetrics.Measure(piece, style.FontFamily, style.FontSize, style.Bold, style.Italic, fontCache, out float[] charWidths, out bool[] canBreakBefore);

						int segStart = 0;
						int lastBreak = -1;
						float widthSinceSegStart = 0f;

						for (int i = 0; i < piece.Length; i++)
						{
							if (i > segStart && canBreakBefore[i])
							{
								lastBreak = i;
							}

							float charWidth = charWidths[i];
							bool overflow = (lineHasContent || i > segStart) && (currentWidth + widthSinceSegStart + charWidth > maxWidthPoints);
							if (overflow && lastBreak > segStart)
							{
								AppendFragment(piece.Substring(segStart, lastBreak - segStart), style);
								FlushLine();
								segStart = lastBreak;
								lastBreak = -1;
								widthSinceSegStart = 0f;
								for (int k = segStart; k <= i; k++)
								{
									widthSinceSegStart += charWidths[k];
								}
								continue;
							}

							widthSinceSegStart += charWidth;
						}

						if (segStart < piece.Length)
						{
							AppendFragment(piece.Substring(segStart), style);
							currentWidth += widthSinceSegStart;
						}
					}

					bool forcedBreakAfterPiece = pieceIndex < pieces.Length - 1;
					if (forcedBreakAfterPiece)
					{
						FlushLine();
					}
				}
			}

			if (lineHasContent || lines.Count == 0)
			{
				lines.Add(currentLine);
			}
			return lines;
		}
	}
}
