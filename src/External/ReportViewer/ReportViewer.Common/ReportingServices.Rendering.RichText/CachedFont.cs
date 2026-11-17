using System;
using System.Drawing;

namespace ReportingServices.Rendering.RichText
{
	internal sealed class CachedFont : IDisposable
	{
		private Win32ObjectSafeHandle m_hfont = Win32ObjectSafeHandle.Zero;

		private Font m_font;

		private Win32.TEXTMETRIC m_textMetric;

		private bool m_initialized;

		private float m_scaleFactor = 1f;

		internal ScriptCacheSafeHandle ScriptCache = new ScriptCacheSafeHandle();

		private short? m_defaultGlyph;

		private SkiaCachedFont m_skiaFont;

		/// <summary>
		/// Set instead of <see cref="Hfont"/>/<see cref="Font"/> when this <see cref="CachedFont"/>
		/// was created on a non-Windows platform (<see cref="FontCache.CreateFont"/>) - metrics
		/// come from SkiaSharp instead of a real HFONT+TEXTMETRIC, so <see cref="GetHeight"/>/
		/// <see cref="GetAscent"/>/<see cref="GetDescent"/>/<see cref="GetLeading"/> read this
		/// instead of calling <see cref="Win32.GetTextMetrics"/>.
		/// </summary>
		internal SkiaCachedFont SkiaFont
		{
			get
			{
				return m_skiaFont;
			}
			set
			{
				m_skiaFont = value;
			}
		}

		internal Win32ObjectSafeHandle Hfont
		{
			get
			{
				return m_hfont;
			}
			set
			{
				m_hfont = value;
			}
		}

		internal Font Font
		{
			get
			{
				return m_font;
			}
			set
			{
				m_font = value;
			}
		}

		public float ScaleFactor
		{
			get
			{
				return m_scaleFactor;
			}
			set
			{
				m_scaleFactor = value;
			}
		}

		internal Win32.TEXTMETRIC TextMetric => m_textMetric;

		internal short? DefaultGlyph
		{
			get
			{
				return m_defaultGlyph;
			}
			set
			{
				m_defaultGlyph = value;
			}
		}

		internal CachedFont()
		{
		}

		private void Dispose(bool disposing)
		{
			if (disposing && m_font != null)
			{
				m_font.Dispose();
				m_font = null;
			}
			if (disposing && m_skiaFont != null)
			{
				m_skiaFont.Dispose();
				m_skiaFont = null;
			}
			if (m_hfont != null && !m_hfont.IsInvalid)
			{
				m_hfont.Close();
				m_hfont = null;
			}
			ScriptCache.Close();
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		~CachedFont()
		{
			Dispose(disposing: false);
		}

		private void Initialize(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			fontCache.SelectFontObject(hdc, m_hfont);
			Win32.GetTextMetrics(hdc, out m_textMetric);
			if (ScaleFactor != 1f)
			{
				m_textMetric.tmHeight = Scale(m_textMetric.tmHeight);
				m_textMetric.tmAscent = Scale(m_textMetric.tmAscent);
				m_textMetric.tmDescent = Scale(m_textMetric.tmDescent);
				m_textMetric.tminternalLeading = Scale(m_textMetric.tminternalLeading);
			}
			m_initialized = true;
		}

		private int Scale(int value)
		{
			return (int)((float)value / ScaleFactor + 0.5f);
		}

		internal int GetHeight(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			if (m_skiaFont != null)
			{
				return m_skiaFont.GetHeight();
			}
			if (!m_initialized)
			{
				Initialize(hdc, fontCache);
			}
			return m_textMetric.tmHeight;
		}

		internal int GetAscent(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			if (m_skiaFont != null)
			{
				return m_skiaFont.GetAscent();
			}
			if (!m_initialized)
			{
				Initialize(hdc, fontCache);
			}
			return m_textMetric.tmAscent;
		}

		internal int GetDescent(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			if (m_skiaFont != null)
			{
				return m_skiaFont.GetDescent();
			}
			if (!m_initialized)
			{
				Initialize(hdc, fontCache);
			}
			return m_textMetric.tmDescent;
		}

		internal int GetLeading(Win32DCSafeHandle hdc, FontCache fontCache)
		{
			if (m_skiaFont != null)
			{
				return m_skiaFont.GetLeading();
			}
			if (!m_initialized)
			{
				Initialize(hdc, fontCache);
			}
			return m_textMetric.tminternalLeading;
		}
	}
}
