using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ReportingServices.Rendering.RichText
{
	internal class TextRun
	{
		protected string m_text;

		protected bool m_clone;

		protected ITextRunProps m_textRunProps;

		protected GlyphData m_cachedGlyphData;

		protected CachedFont m_cachedFont;

		protected ScriptAnalysis m_scriptAnalysis;

		internal SCRIPT_ANALYSIS SCRIPT_ANALYSIS;

		internal SCRIPT_LOGATTR[] ScriptLogAttr;

		internal int m_underlineHeight;

		private int? m_itemizedScriptId;

		private TextRunState m_runState;

		public bool IsComplex
		{
			get
			{
				if (ScriptProperties.GetProperties(ScriptAnalysis.eScript).IsComplex)
				{
					return true;
				}
				return false;
			}
		}

		internal ITextRunProps TextRunProperties => m_textRunProps;

		internal CachedFont CachedFont => m_cachedFont;

		internal TextRunState State => m_runState;

		internal int? ItemizedScriptId => m_itemizedScriptId;

		internal bool FallbackFont
		{
			get
			{
				return (int)(m_runState & TextRunState.FallbackFont) > 0;
			}
			set
			{
				if (value)
				{
					m_runState |= TextRunState.FallbackFont;
				}
				else
				{
					m_runState &= ~TextRunState.FallbackFont;
				}
			}
		}

		internal bool HasEastAsianChars
		{
			get
			{
				return (int)(m_runState & TextRunState.HasEastAsianChars) > 0;
			}
			set
			{
				if (value)
				{
					m_runState |= TextRunState.HasEastAsianChars;
				}
				else
				{
					m_runState &= ~TextRunState.HasEastAsianChars;
				}
			}
		}

		internal int CharacterCount => m_text.Length;

		internal uint ColorInt
		{
			get
			{
				Color color = m_textRunProps.Color;
				return (uint)((color.B << 16) | (color.G << 8) | color.R);
			}
		}

		internal ScriptAnalysis ScriptAnalysis
		{
			get
			{
				if (m_scriptAnalysis == null)
				{
					m_scriptAnalysis = new ScriptAnalysis(SCRIPT_ANALYSIS.word1);
					m_scriptAnalysis.s = new ScriptState(SCRIPT_ANALYSIS.state.word1);
				}
				return m_scriptAnalysis;
			}
		}

		internal string Text => m_text;

		internal GlyphData GlyphData => m_cachedGlyphData;

		internal bool Clone
		{
			get
			{
				return m_clone;
			}
			set
			{
				m_clone = value;
			}
		}

		internal virtual int HighlightStart
		{
			get
			{
				return -1;
			}
			set
			{
			}
		}

		internal virtual int HighlightEnd
		{
			get
			{
				return -1;
			}
			set
			{
			}
		}

		internal virtual Color HighlightColor
		{
			get
			{
				return Color.Empty;
			}
			set
			{
			}
		}

		internal virtual int CharacterIndexInOriginal
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		internal virtual bool IsHighlightTextRun => false;

		internal virtual bool IsPlaceholderTextRun => false;

		internal int UnderlineHeight
		{
			get
			{
				return m_underlineHeight;
			}
			set
			{
				m_underlineHeight = value;
			}
		}

		internal virtual bool AllowColorInversion
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		internal TextRun(string text, ITextRunProps props)
		{
			bool hasEastAsianChars = false;
			m_text = Utilities.ConvertTabAndCheckEastAsianChars(text, out hasEastAsianChars);
			HasEastAsianChars = hasEastAsianChars;
			m_textRunProps = props;
		}

		internal TextRun(string text, TextRun textRun)
			: this(text, textRun.TextRunProperties)
		{
		}

		internal TextRun(string text, ITextRunProps props, TexRunShapeData shapeData)
			: this(text, props)
		{
			if (shapeData != null)
			{
				SCRIPT_ANALYSIS = shapeData.Analysis;
				ScriptLogAttr = shapeData.ScriptLogAttr;
				m_cachedFont = shapeData.Font;
				m_itemizedScriptId = shapeData.ItemizedScriptId;
				m_runState = shapeData.State;
				// shapeData.GlyphData only ever holds the Windows ScriptShape step's output
				// (glyphs/clusters/VisAttrs) - ScriptPlace still has to run later, lazily,
				// against a real HDC (see LoadGlyphData). The cross-platform HarfBuzz path has
				// no such shape/place split: SKShaper.Shape produces both together in one call,
				// so there's no placement step to defer and nothing usable to reuse here.
				// Leaving m_cachedGlyphData null makes the first GetWidth/GetGlyphData call
				// fall through to ShapeAndPlace -> ShapeAndPlaceCrossPlatform and re-shape for
				// real, instead of LoadGlyphData trying to Win32.ScriptPlace non-existent
				// placement data on a platform that has no such API.
				if (shapeData.GlyphData != null && OperatingSystem.IsWindows())
				{
					m_cachedGlyphData = new GlyphData(shapeData.GlyphData);
				}
			}
		}

		internal TextRun(string text, TextRun textRun, SCRIPT_LOGATTR[] scriptLogAttr)
			: this(text, textRun)
		{
			SCRIPT_ANALYSIS = textRun.SCRIPT_ANALYSIS;
			ScriptLogAttr = scriptLogAttr;
			m_cachedFont = textRun.CachedFont;
			m_itemizedScriptId = textRun.ItemizedScriptId;
			bool hasEastAsianChars = HasEastAsianChars;
			m_runState = textRun.State;
			HasEastAsianChars = hasEastAsianChars;
		}

		internal virtual TextRun Split(string text, SCRIPT_LOGATTR[] scriptLogAttr)
		{
			return new TextRun(text, this, scriptLogAttr);
		}

		internal virtual TextRun GetSubRun(int startIndex, int length)
		{
			if (length == m_text.Length)
			{
				return this;
			}
			if (startIndex > 0)
			{
				m_textRunProps.AddSplitIndex(startIndex);
			}
			return new TextRun(m_text.Substring(startIndex, length), this);
		}

		internal TextRun GetSubRun(int startIndex)
		{
			if (startIndex == 0)
			{
				return this;
			}
			return GetSubRun(startIndex, m_text.Length - startIndex);
		}

		internal int[] GetLogicalWidths(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			GlyphData glyphData = GetGlyphData(hdc, fontCache);
			if (!OperatingSystem.IsWindows())
			{
				return GetLogicalWidthsCrossPlatform(glyphData);
			}
			int[] array = new int[m_text.Length];
			int num = Win32.ScriptGetLogicalWidths(ref SCRIPT_ANALYSIS, m_text.Length, glyphData.GlyphScriptShapeData.GlyphCount, glyphData.ScaledAdvances, glyphData.GlyphScriptShapeData.Clusters, glyphData.GlyphScriptShapeData.VisAttrs, array);
			if (Win32.Failed(num))
			{
				Marshal.ThrowExceptionForHR(num);
			}
			if (glyphData.Scaled)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = glyphData.Scale(array[i]);
				}
			}
			return array;
		}

		/// <summary>
		/// Cross-platform counterpart to <see cref="GetLogicalWidths"/>'s Win32
		/// ScriptGetLogicalWidths call - converts per-glyph advances back into
		/// per-character widths using <see cref="HarfBuzzTextShaper"/>'s cluster mapping
		/// (<see cref="GlyphShapeData.Clusters"/>: character index -&gt; owning glyph
		/// index) instead of Uniscribe. Backs <see cref="LineBreaker"/>'s character-level
		/// line-fold path (<c>FindWidthToBreakPosition</c>/<c>FindFoldTextPosition_CharacterTrim</c>),
		/// hit whenever a line needs to break mid-run rather than at a run boundary.
		///
		/// For a simple 1-char-per-glyph cluster (the common case for plain Latin text)
		/// this returns exactly that glyph's advance. For a multi-character cluster (a
		/// ligature, or several combining characters shaped into one glyph) the glyph's
		/// total advance is split evenly across its characters, with any remainder from
		/// integer division added to the cluster's last character - an approximation,
		/// not a byte-for-byte port of Uniscribe's own splitting rule, but one that
		/// preserves the invariant every caller actually needs: summing this array over
		/// any prefix of the text yields (up to rounding) that prefix's real shaped width.
		/// Exposed as its own internal method (not just inlined in <see cref="GetLogicalWidths"/>)
		/// so tests can exercise it directly regardless of host OS, matching this
		/// codebase's existing convention for platform-gated RichText code.
		/// </summary>
		internal int[] GetLogicalWidthsCrossPlatform(GlyphData glyphData)
		{
			int[] result = new int[m_text.Length];
			if (m_text.Length == 0)
			{
				return result;
			}
			short[] clusters = glyphData.GlyphScriptShapeData.Clusters;
			int[] advances = glyphData.RawAdvances;
			int i = 0;
			while (i < m_text.Length)
			{
				short glyphIndex = clusters[i];
				int clusterEnd = i;
				while (clusterEnd + 1 < m_text.Length && clusters[clusterEnd + 1] == glyphIndex)
				{
					clusterEnd++;
				}
				int clusterLength = clusterEnd - i + 1;
				int glyphAdvance = (glyphIndex >= 0 && glyphIndex < advances.Length) ? advances[glyphIndex] : 0;
				int perCharWidth = glyphAdvance / clusterLength;
				for (int c = i; c <= clusterEnd; c++)
				{
					result[c] = perCharWidth;
				}
				result[clusterEnd] += glyphAdvance - perCharWidth * clusterLength;
				i = clusterEnd + 1;
			}
			if (glyphData.Scaled)
			{
				for (int j = 0; j < result.Length; j++)
				{
					result[j] = glyphData.Scale(result[j]);
				}
			}
			return result;
		}

		internal void TerminateAt(int index)
		{
			m_text = m_text.Remove(index, m_text.Length - index);
			SCRIPT_LOGATTR[] array = new SCRIPT_LOGATTR[index];
			Array.Copy(ScriptLogAttr, 0, array, 0, array.Length);
			ScriptLogAttr = array;
			m_cachedGlyphData = null;
		}

		internal CachedFont GetCachedFont(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			if (m_cachedGlyphData == null)
			{
				ShapeAndPlace(hdc, fontCache);
			}
			else
			{
				LoadGlyphData(hdc, fontCache);
			}
			return m_cachedFont;
		}

		internal GlyphData GetGlyphData(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			if (m_cachedGlyphData == null)
			{
				ShapeAndPlace(hdc, fontCache);
			}
			else
			{
				LoadGlyphData(hdc, fontCache);
			}
			return m_cachedGlyphData;
		}

		internal int GetWidth(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			return GetWidth(hdc, fontCache, isAtLineEnd: false);
		}

		internal int GetWidth(Win32DCSafeHandle hdc, FontCache fontCache, bool isAtLineEnd)
		{
			if (m_cachedGlyphData == null)
			{
				ShapeAndPlace(hdc, fontCache);
			}
			else
			{
				LoadGlyphData(hdc, fontCache);
			}
			return m_cachedGlyphData.GetTotalWidth(isAtLineEnd);
		}

		internal int GetHeight(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			return GetCachedFont(hdc, fontCache).GetHeight(hdc, fontCache);
		}

		internal int GetAscent(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			return GetCachedFont(hdc, fontCache).GetAscent(hdc, fontCache);
		}

		internal int GetDescent(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			return GetCachedFont(hdc, fontCache).GetDescent(hdc, fontCache);
		}

		internal void ShapeAndPlace(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			if (!OperatingSystem.IsWindows())
			{
				ShapeAndPlaceCrossPlatform(fontCache);
				return;
			}
			bool verticalFont = false;
			if (fontCache.AllowVerticalFont)
			{
				verticalFont = HasEastAsianChars;
			}
			if (m_cachedFont == null)
			{
				m_cachedFont = fontCache.GetFont(m_textRunProps, GetCharset(), verticalFont);
				FallbackFont = false;
			}
			CachedFont cachedFont = m_cachedFont;
			bool flag = false;
			bool flag2 = false;
			string text = m_text;
			int num = Convert.ToInt32((double)text.Length * 1.5 + 16.0);
			m_cachedGlyphData = new GlyphData(num, text.Length);
			GlyphShapeData glyphScriptShapeData = m_cachedGlyphData.GlyphScriptShapeData;
			int num2 = Win32.ScriptShape(IntPtr.Zero, ref m_cachedFont.ScriptCache, text, text.Length, num, ref SCRIPT_ANALYSIS, glyphScriptShapeData.Glyphs, glyphScriptShapeData.Clusters, glyphScriptShapeData.VisAttrs, ref glyphScriptShapeData.GlyphCount);
			if (num2 == -2147483638)
			{
				flag = true;
				fontCache.SelectFontObject(hdc, m_cachedFont.Hfont);
				num2 = Win32.ScriptShape(hdc, ref m_cachedFont.ScriptCache, text, text.Length, num, ref SCRIPT_ANALYSIS, glyphScriptShapeData.Glyphs, glyphScriptShapeData.Clusters, glyphScriptShapeData.VisAttrs, ref glyphScriptShapeData.GlyphCount);
			}
			if (num2 == -2147024882)
			{
				num = text.Length * 3;
				m_cachedGlyphData = new GlyphData(num, text.Length);
				glyphScriptShapeData = m_cachedGlyphData.GlyphScriptShapeData;
				num2 = Win32.ScriptShape(hdc, ref m_cachedFont.ScriptCache, text, text.Length, num, ref SCRIPT_ANALYSIS, glyphScriptShapeData.Glyphs, glyphScriptShapeData.Clusters, glyphScriptShapeData.VisAttrs, ref glyphScriptShapeData.GlyphCount);
			}
			if (!FallbackFont)
			{
				if (num2 == -2147220992)
				{
					m_cachedFont = fontCache.GetFallbackFont(script: (!m_itemizedScriptId.HasValue) ? ScriptAnalysis.eScript : m_itemizedScriptId.Value, textRunProps: m_textRunProps, charset: GetCharset(), verticalFont: verticalFont);
					fontCache.SelectFontObject(hdc, m_cachedFont.Hfont);
					flag = true;
					flag2 = true;
					num2 = Win32.ScriptShape(hdc, ref m_cachedFont.ScriptCache, text, text.Length, num, ref SCRIPT_ANALYSIS, glyphScriptShapeData.Glyphs, glyphScriptShapeData.Clusters, glyphScriptShapeData.VisAttrs, ref glyphScriptShapeData.GlyphCount);
				}
				else if (HasEastAsianChars)
				{
					if (!flag)
					{
						fontCache.SelectFontObject(hdc, m_cachedFont.Hfont);
						flag = true;
					}
					Win32.SCRIPT_FONTPROPERTIES sfp = default(Win32.SCRIPT_FONTPROPERTIES);
					sfp.cBytes = 16;
					num2 = Win32.ScriptGetFontProperties(hdc, ref m_cachedFont.ScriptCache, ref sfp);
					short wgDefault = sfp.wgDefault;
					int num4 = 0;
					num4 = ((!m_itemizedScriptId.HasValue) ? ScriptAnalysis.eScript : m_itemizedScriptId.Value);
					for (int i = 0; i < glyphScriptShapeData.GlyphCount; i++)
					{
						if (glyphScriptShapeData.Glyphs[i] == wgDefault)
						{
							m_cachedFont = fontCache.GetFallbackFont(m_textRunProps, GetCharset(), num4, verticalFont);
							m_cachedFont.DefaultGlyph = wgDefault;
							fontCache.SelectFontObject(hdc, m_cachedFont.Hfont);
							flag = true;
							flag2 = true;
							num2 = Win32.ScriptShape(hdc, ref m_cachedFont.ScriptCache, text, text.Length, num, ref SCRIPT_ANALYSIS, glyphScriptShapeData.Glyphs, glyphScriptShapeData.Clusters, glyphScriptShapeData.VisAttrs, ref glyphScriptShapeData.GlyphCount);
							break;
						}
					}
				}
			}
			if (num2 == -2147220992)
			{
				m_cachedFont = cachedFont;
				if (!flag || flag2)
				{
					Win32.SelectObject(hdc, m_cachedFont.Hfont).SetHandleAsInvalid();
					flag = true;
				}
				flag2 = false;
				SetUndefinedScript();
				num2 = Win32.ScriptShape(hdc, ref m_cachedFont.ScriptCache, text, text.Length, num, ref SCRIPT_ANALYSIS, glyphScriptShapeData.Glyphs, glyphScriptShapeData.Clusters, glyphScriptShapeData.VisAttrs, ref glyphScriptShapeData.GlyphCount);
			}
			if (Win32.Failed(num2))
			{
				Marshal.ThrowExceptionForHR(num2);
			}
			if (flag2)
			{
				FallbackFont = true;
			}
			m_cachedGlyphData.TrimToGlyphCount();
			m_cachedGlyphData.ScaleFactor = m_cachedFont.ScaleFactor;
			TextScriptPlace(hdc, flag, fontCache);
		}

		/// <summary>
		/// Cross-platform counterpart to <see cref="ShapeAndPlace"/> - shapes via
		/// <see cref="HarfBuzzTextShaper"/>/<see cref="SkiaCachedFont"/> instead of
		/// Win32's ScriptShape/ScriptPlace, so no HDC/HFONT/GDI+ <see cref="System.Drawing.Font"/>
		/// is ever constructed. Per tasks/pdf-text-shaping-abstraction.md's documented
		/// scope: no vertical-font support, charset is not used to select a Skia typeface
		/// (SkiaSharp resolves by family name/style only) - <see cref="GetCharset"/> is
		/// deliberately not called here, since <see cref="ScriptProperties"/>'s static
		/// constructor itself P/Invokes Win32.ScriptGetProperties and would crash on first
		/// touch on this platform.
		///
		/// Fallback-font retry (mirrors <see cref="ShapeAndPlace"/>'s own .notdef retry via
		/// <see cref="FontCache.GetFallbackFont"/>): if the resolved font is missing a real
		/// glyph for some character (reported by <see cref="HarfBuzzTextShaper.Shape(string, SkiaCachedFont, out int)"/>),
		/// re-shape once against whatever system font <see cref="FontCache.GetFallbackFontCrossPlatform"/>
		/// resolves for that missing codepoint - e.g. a CJK character requested against a
		/// Latin-only font family. Only retried once per run (not once per still-missing
		/// glyph after the retry) - matches the Win32 path's own single-retry shape, and
		/// keeps this bounded even if the fallback font itself is incomplete.
		/// </summary>
		internal void ShapeAndPlaceCrossPlatform(FontCache fontCache)
		{
			if (m_cachedFont == null)
			{
				m_cachedFont = fontCache.GetFontCrossPlatform(m_textRunProps);
				FallbackFont = false;
			}
			m_cachedGlyphData = HarfBuzzTextShaper.Shape(m_text, m_cachedFont.SkiaFont, out int missingGlyphCodepoint);
			if (!FallbackFont && missingGlyphCodepoint >= 0)
			{
				CachedFont fallbackFont = fontCache.GetFallbackFontCrossPlatform(m_textRunProps, missingGlyphCodepoint);
				if (fallbackFont != null)
				{
					m_cachedFont = fallbackFont;
					FallbackFont = true;
					m_cachedGlyphData = HarfBuzzTextShaper.Shape(m_text, m_cachedFont.SkiaFont);
				}
			}
			m_cachedGlyphData.NeedGlyphPlaceData = false;
			m_cachedGlyphData.ScaleFactor = m_cachedFont.ScaleFactor;
		}

		private void TextScriptPlace(Win32DCSafeHandle hdc, bool fontSelected, FontCache fontCache)
		{
			int num = 0;
			GlyphShapeData glyphScriptShapeData = m_cachedGlyphData.GlyphScriptShapeData;
			if (fontSelected)
			{
				num = Win32.ScriptPlace(hdc, ref m_cachedFont.ScriptCache, glyphScriptShapeData.Glyphs, glyphScriptShapeData.GlyphCount, glyphScriptShapeData.VisAttrs, ref SCRIPT_ANALYSIS, m_cachedGlyphData.RawAdvances, m_cachedGlyphData.RawGOffsets, ref m_cachedGlyphData.ABC);
			}
			else
			{
				num = Win32.ScriptPlace(IntPtr.Zero, ref m_cachedFont.ScriptCache, glyphScriptShapeData.Glyphs, glyphScriptShapeData.GlyphCount, glyphScriptShapeData.VisAttrs, ref SCRIPT_ANALYSIS, m_cachedGlyphData.RawAdvances, m_cachedGlyphData.RawGOffsets, ref m_cachedGlyphData.ABC);
				if (num == -2147483638)
				{
					fontCache.SelectFontObject(hdc, m_cachedFont.Hfont);
					num = Win32.ScriptPlace(hdc, ref m_cachedFont.ScriptCache, glyphScriptShapeData.Glyphs, glyphScriptShapeData.GlyphCount, glyphScriptShapeData.VisAttrs, ref SCRIPT_ANALYSIS, m_cachedGlyphData.RawAdvances, m_cachedGlyphData.RawGOffsets, ref m_cachedGlyphData.ABC);
				}
			}
			if (Win32.Failed(num))
			{
				Marshal.ThrowExceptionForHR(num);
			}
			if (m_cachedGlyphData.ABC.Width > 0 && m_text.Length == 1 && TextBox.IsWhitespaceControlChar(m_text[0]))
			{
				m_cachedGlyphData.ABC.SetToZeroWidth();
			}
		}

		private void LoadGlyphData(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			if (!m_clone && m_cachedGlyphData.NeedGlyphPlaceData)
			{
				m_cachedGlyphData.NeedGlyphPlaceData = false;
				m_cachedGlyphData.ScaleFactor = m_cachedFont.ScaleFactor;
				TextScriptPlace(hdc, fontSelected: false, fontCache);
			}
		}

		private void SetUndefinedScript()
		{
			ScriptAnalysis scriptAnalysis = ScriptAnalysis;
			if (scriptAnalysis.eScript != 0)
			{
				m_itemizedScriptId = scriptAnalysis.eScript;
				scriptAnalysis.eScript = 0;
				SCRIPT_ANALYSIS = scriptAnalysis.GetAs_SCRIPT_ANALYSIS();
				m_scriptAnalysis = null;
			}
		}

		private byte GetCharset()
		{
			byte result = 1;
			ScriptProperties properties = ScriptProperties.GetProperties(ScriptAnalysis.eScript);
			if (!properties.IsComplex)
			{
				SetUndefinedScript();
			}
			else if (!properties.IsAmbiguousCharSet)
			{
				result = properties.CharSet;
			}
			return result;
		}
	}
}
