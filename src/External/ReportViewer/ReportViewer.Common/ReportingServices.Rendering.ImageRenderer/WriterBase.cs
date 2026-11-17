using ReportingServices.Interfaces;
using ReportingServices.Rendering.RichText;
using ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ReportingServices.Rendering.ImageRenderer
{
	internal abstract class WriterBase : IDisposable
	{
		private const int DPI = 96;

		protected Stream m_outputStream;

		protected GraphicsBase m_commonGraphics;

		protected Dictionary<string, GDIFont> m_gdiFonts = new Dictionary<string, GDIFont>();

		protected Renderer m_renderer;

		private bool m_disposeRenderer = true;

		private RectangleF m_pageSectionBounds = RectangleF.Empty;

		private CreateAndRegisterStream m_createAndRegisterStream;

		internal RectangleF PageSectionBounds => m_pageSectionBounds;

		internal virtual float HalfPixelWidthY => ConvertToMillimeters(1) / 2f;

		internal virtual float HalfPixelWidthX => ConvertToMillimeters(1) / 2f;

		internal CreateAndRegisterStream CreateAndRegisterStream => m_createAndRegisterStream;

		internal GraphicsBase CommonGraphics => m_commonGraphics;

		protected WriterBase(Renderer renderer, Stream stream, bool disposeRenderer, CreateAndRegisterStream createAndRegisterStream)
		{
			m_renderer = renderer;
			m_outputStream = stream;
			m_disposeRenderer = disposeRenderer;
			m_createAndRegisterStream = createAndRegisterStream;
			if (m_renderer != null)
			{
				m_renderer.Writer = this;
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}
			if (m_commonGraphics != null)
			{
				m_commonGraphics.Dispose();
				m_commonGraphics = null;
			}
			if (m_gdiFonts != null)
			{
				foreach (string key in m_gdiFonts.Keys)
				{
					m_gdiFonts[key].Dispose();
				}
				m_gdiFonts = null;
			}
			if (m_disposeRenderer && m_renderer != null)
			{
				m_renderer.Dispose();
				m_renderer = null;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		~WriterBase()
		{
			Dispose(disposing: false);
		}

		internal virtual void BeginReport(int dpiX, int dpiY)
		{
			if (dpiX == 0)
			{
				dpiX = 96;
			}
			if (dpiY == 0)
			{
				dpiY = 96;
			}
			m_commonGraphics = new GraphicsBase(dpiX, dpiY);
		}

		internal virtual void BeginPage(float pageWidth, float pageHeight)
		{
		}

		internal virtual void BeginPageSection(RectangleF bounds)
		{
			m_pageSectionBounds = bounds;
		}

		internal virtual RectangleF CalculatePageBounds(RPLPageContent pageContent, out float pageWidth, out float pageHeight)
		{
			RPLPageLayout pageLayout = pageContent.PageLayout;
			pageWidth = pageLayout.PageWidth;
			pageHeight = pageLayout.PageHeight;
			float width = pageLayout.PageWidth - pageLayout.MarginLeft - pageLayout.MarginRight;
			float height = pageLayout.PageHeight - pageLayout.MarginTop - pageLayout.MarginBottom;
			return new RectangleF(pageLayout.MarginLeft, pageLayout.MarginTop, width, height);
		}

		internal virtual RectangleF CalculateColumnBounds(RPLReportSection section, RPLPageLayout pageLayout, RPLItemMeasurement column, int columnNumber, float top, float columnHeight, float columnWidth)
		{
			return RectangleF.Empty;
		}

		internal virtual RectangleF CalculateHeaderBounds(RPLReportSection section, RPLPageLayout pageLayout, float top, float width)
		{
			return RectangleF.Empty;
		}

		internal virtual RectangleF CalculateFooterBounds(RPLReportSection section, RPLPageLayout pageLayout, float top, float width)
		{
			return RectangleF.Empty;
		}

		internal virtual void DrawBackgroundImage(RPLImageData imageData, RPLFormat.BackgroundRepeatTypes repeat, PointF start, RectangleF position)
		{
		}

		internal virtual void DrawLine(Color color, float size, RPLFormat.BorderStyles style, float x1, float y1, float x2, float y2)
		{
		}

		internal virtual void DrawDynamicImage(string imageName, Stream imageStream, long imageDataOffset, RectangleF position)
		{
		}

		internal virtual void DrawImage(RectangleF position, RPLImage image, RPLImageProps instanceProperties, RPLImagePropsDef definitionProperties)
		{
		}

		internal virtual void DrawRectangle(Color color, float size, RPLFormat.BorderStyles style, RectangleF rectangle)
		{
		}

		internal virtual void DrawTextRun(Win32DCSafeHandle hdc, FontCache fontCache, ReportTextBox textBox, TextRun run, TypeCode typeCode, RPLFormat.TextAlignments textAlign, RPLFormat.VerticalAlignments verticalAlign, RPLFormat.WritingModes writingMode, RPLFormat.Directions direction, Point position, Rectangle layoutRectangle, int lineHeight, int baselineY)
		{
		}

		/// <summary>
		/// Cross-platform text drawing hook: renders a plain, single-style, left-to-right
		/// text box without going through RichText's LineBreaker/TextBox/FontCache/Win32
		/// Uniscribe pipeline at all (see tasks/pdf-text-shaping-abstraction.md - that
		/// pipeline cannot run at all on non-Windows platforms, not even for HDC-free
		/// measurement). Only meaningfully implemented by PDFWriter; no-op by default
		/// (the Windows-only ImageWriter/EMF path is unaffected and keeps using
		/// DrawTextRun above via the existing LineBreaker/TextBox path).
		/// </summary>
		internal virtual void DrawWrappedText(RectangleF textPosition, PointF offset, string text, ITextRunProps style, RPLFormat.TextAlignments alignment)
		{
		}

		/// <summary>
		/// Cross-platform counterpart to DrawWrappedText for multi-run ("rich text")
		/// text boxes - see tasks/pdf-text-shaping-abstraction.md. Each paragraph carries
		/// its own resolved alignment (RPLFormat.TextAlignments.General already resolved
		/// against the text box's DefaultAlignment/Direction by the caller) and its runs
		/// in order, each run a (text, style) pair. No-op by default.
		/// </summary>
		internal virtual void DrawWrappedRichText(RectangleF textPosition, PointF offset, List<(RPLFormat.TextAlignments Alignment, List<(string Text, ITextRunProps Style)> Runs)> paragraphs)
		{
		}

		internal virtual void EndPageSection()
		{
		}

		internal virtual void EndPage()
		{
		}

		internal virtual void EndReport()
		{
		}

		internal virtual void FillPolygon(Color color, PointF[] polygon)
		{
		}

		internal virtual void FillRectangle(Color color, RectangleF rectangle)
		{
		}

		internal virtual void PostProcessPage()
		{
		}

		internal virtual void PostProcessReportItem(object state)
		{
		}

		internal virtual void PreProcessPage(string uniqueName, RectangleF bounds)
		{
		}

		internal virtual object PreProcessReportItem(RPLElement element, RPLElementProps instanceProperties, RectangleF position, bool hasLabel)
		{
			return null;
		}

		internal virtual void ProcessAction(string uniqueName, RPLActionInfo actionInfo, RectangleF position)
		{
		}

		internal virtual void ProcessAction(string uniqueName, RPLActionInfoWithImageMap actionInfo, RectangleF position)
		{
		}

		internal virtual void ProcessBookmark(string uniqueName, PointF point)
		{
		}

		internal virtual void ProcessFixedHeaders(RPLTablix tablix, RectangleF position, float[] rowStarts, float[] columnStarts)
		{
		}

		internal virtual void ProcessLabel(string uniqueName, string label, PointF point)
		{
		}

		internal virtual void ProcessSort(string uniqueName, RPLFormat.SortOptions sortState, ref RectangleF textPosition)
		{
		}

		internal virtual void ProcessToggle(string uniqueName, bool toggleState, ref RectangleF textPosition)
		{
		}

		internal void GetHdc(bool createNewHdc, out Win32DCSafeHandle hdc, out float dpiX)
		{
			if (m_commonGraphics != null)
			{
				m_commonGraphics.CacheHdc(createNewHdc);
				hdc = m_commonGraphics.Hdc;
				dpiX = m_commonGraphics.DpiX;
			}
			else
			{
				hdc = Win32DCSafeHandle.Zero;
				dpiX = -1f;
			}
		}

		internal void ReleaseHdc(bool release)
		{
			if (m_commonGraphics != null)
			{
				m_commonGraphics.ReleaseCachedHdc(release);
			}
		}

		internal virtual void ClipTextboxRectangle(Win32DCSafeHandle hdc, RectangleF position)
		{
		}

		/// <summary>
		/// Whether this writer's RichText.TextBox/LineBreaker orchestration (itemize/shape/
		/// measure/draw via DrawTextRun above) can run on non-Windows platforms. False by
		/// default: ImageWriter's GDI+/EMF output still requires a real Win32 HDC end to end.
		/// PDFWriter overrides this to true once its own DrawTextRun/ProcessDrawStringFont
		/// no longer need one - see tasks/pdf-text-shaping-abstraction.md.
		/// </summary>
		internal virtual bool SupportsCrossPlatformRichTextPipeline => false;

		internal virtual void UnClipTextboxRectangle(Win32DCSafeHandle hdc)
		{
		}

		internal virtual float ConvertToMillimeters(int pixels)
		{
			return m_commonGraphics.ConvertToMillimeters(pixels);
		}

		internal virtual int ConvertToPixels(float mm)
		{
			return m_commonGraphics.ConvertToPixels(mm);
		}
	}
}
