using Reporting.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Reporting.Map.WebForms
{
	[TypeConverter(typeof(AutoSizePanelConverter))]
	internal class ColorSwatchPanel : AutoSizePanel
	{
		private const int DummyItemsCount = 5;

		private int PanelPadding = 8;

		private int TitleSeparatorSize = 8;

		private int TickMarkLabelGapSize = 2;

		private float TrimmingProtector;

		private bool showSelectedTitle;

		private RectangleF titlePosition;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private Color outlineColor = Color.DimGray;

		private Color labelColor = Color.Black;

		private LabelAlignment labelAlignment = LabelAlignment.Alternate;

		private Color rangeGapsColor = Color.White;

		private string numricLabelFormat = "#,##0.##";

		private SwatchLabelType labelType;

		private bool showEndLabels = true;

		private int tickMarkLength = 3;

		private int labelInterval = 1;

		private SwatchColorCollection swatchColors;

		private string noDataText = "N/A";

		private Font titleFont = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Bold);

		private Color titleColor = Color.Black;

		private StringAlignment titleAlignment = StringAlignment.Center;

		private string title = "";

		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		[Browsable(false)]
		internal bool ShowSelectedTitle
		{
			get
			{
				return showSelectedTitle;
			}
			set
			{
				showSelectedTitle = value;
				Invalidate(layout: true);
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		[Browsable(false)]
		internal RectangleF TitleSelectionRectangle
		{
			get
			{
				RectangleF result = titlePosition;
				result.Offset(GetAbsoluteLocation());
				return result;
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_Font")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt")]
		public Font Font
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
				Invalidate(layout: true);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_OutlineColor")]
		[DefaultValue(typeof(Color), "DimGray")]
		public Color OutlineColor
		{
			get
			{
				return outlineColor;
			}
			set
			{
				outlineColor = value;
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_LabelColor")]
		[DefaultValue(typeof(Color), "Black")]
		public Color LabelColor
		{
			get
			{
				return labelColor;
			}
			set
			{
				labelColor = value;
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_LabelAlignment")]
		[DefaultValue(LabelAlignment.Alternate)]
		public LabelAlignment LabelAlignment
		{
			get
			{
				return labelAlignment;
			}
			set
			{
				labelAlignment = value;
				Invalidate(layout: true);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_RangeGapColor")]
		[DefaultValue(typeof(Color), "White")]
		public Color RangeGapColor
		{
			get
			{
				return rangeGapsColor;
			}
			set
			{
				rangeGapsColor = value;
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_NumericLabelFormat")]
		[DefaultValue("#,##0.##")]
		public string NumericLabelFormat
		{
			get
			{
				return numricLabelFormat;
			}
			set
			{
				numricLabelFormat = value;
				Invalidate(layout: true);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_LabelType")]
		[DefaultValue(SwatchLabelType.Auto)]
		public SwatchLabelType LabelType
		{
			get
			{
				return labelType;
			}
			set
			{
				labelType = value;
				Invalidate(layout: true);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_ShowEndLabels")]
		[DefaultValue(true)]
		public bool ShowEndLabels
		{
			get
			{
				return showEndLabels;
			}
			set
			{
				showEndLabels = value;
				Invalidate(layout: true);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_TickMarkLength")]
		[DefaultValue(3)]
		public int TickMarkLength
		{
			get
			{
				return tickMarkLength;
			}
			set
			{
				tickMarkLength = value;
				Invalidate(layout: true);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_LabelInterval")]
		[DefaultValue(1)]
		public int LabelInterval
		{
			get
			{
				return labelInterval;
			}
			set
			{
				labelInterval = value;
				Invalidate(layout: true);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_Colors")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SwatchColorCollection Colors
		{
			get
			{
				if (swatchColors == null)
				{
					swatchColors = new SwatchColorCollection(this, common);
				}
				return swatchColors;
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Data")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_NoDataText")]
		[DefaultValue("N/A")]
		public string NoDataText
		{
			get
			{
				return noDataText;
			}
			set
			{
				noDataText = value;
				Invalidate(layout: true);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Title")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_TitleFont")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt, style=Bold")]
		public Font TitleFont
		{
			get
			{
				return titleFont;
			}
			set
			{
				titleFont = value;
				Invalidate(layout: true);
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Title")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_TitleColor")]
		[DefaultValue(typeof(Color), "Black")]
		public Color TitleColor
		{
			get
			{
				return titleColor;
			}
			set
			{
				titleColor = value;
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Title")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_TitleAlignment")]
		[DefaultValue(StringAlignment.Center)]
		public StringAlignment TitleAlignment
		{
			get
			{
				return titleAlignment;
			}
			set
			{
				titleAlignment = value;
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Title")]
		[SRDescription("DescriptionAttributeColorSwatchPanel_Title")]
		[DefaultValue("")]
		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				title = value;
				Invalidate(layout: true);
			}
		}

		internal override bool IsEmpty
		{
			get
			{
				if (Common != null && Common.MapCore.IsDesignMode())
				{
					return false;
				}
				return Colors.Count == 0;
			}
		}

		public override RectangleF GetSelectionRectangle(MapGraphics g, RectangleF clipRect)
		{
			RectangleF result = base.GetSelectionRectangle(g, clipRect);
			if (ShowSelectedTitle && !TitleSelectionRectangle.IsEmpty)
			{
				result = TitleSelectionRectangle;
			}
			return result;
		}

		public ColorSwatchPanel()
			: this(null)
		{
		}

		internal ColorSwatchPanel(CommonElements common)
			: base(common)
		{
			Name = "ColorSwatchPanel";
			MaxAutoSize = 100f;
		}

		public SwatchLabelType GetLabelType()
		{
			if (LabelType != 0)
			{
				return LabelType;
			}
			SwatchLabelType result = SwatchLabelType.ShowBorderValue;
			foreach (SwatchColor color in Colors)
			{
				if (color.HasTextValue)
				{
					return SwatchLabelType.ShowMiddleValue;
				}
			}
			return result;
		}

		internal override void Render(MapGraphics g)
		{
			base.Render(g);
			if (IsEmpty)
			{
				return;
			}
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
			bool flag = false;
			try
			{
				if (Colors.Count == 0 && Common != null && Common.MapCore.IsDesignMode())
				{
					PopulateDummyData();
					flag = true;
				}
				int num = (LabelAlignment != LabelAlignment.Alternate) ? 1 : 2;
				SwatchLabelType swatchLabelType = GetLabelType();
				SizeF colorBoxSize = default(SizeF);
				SizeF labelBoxSize = default(SizeF);
				SizeF firstCaptionSize = default(SizeF);
				SizeF lastCaptionSize = default(SizeF);
				CalculateFontDependentData(g, absoluteRectangle.Size);
				absoluteRectangle.Inflate(-PanelPadding, -PanelPadding);
				if (absoluteRectangle.Width < 1f || absoluteRectangle.Height < 1f)
				{
					return;
				}
				int[] colorsRef = GetColorsRef(swatchLabelType);
				float num2 = 0f;
				if (LabelInterval > 0 && ShowEndLabels)
				{
					firstCaptionSize = g.MeasureString(GetLabelCaption(0, getFromValue: true, swatchLabelType), g.ResourceFactory.WrapFont(Font), absoluteRectangle.Size, g.ResourceFactory.CreateTypographicTextFormat());
					firstCaptionSize.Width += TrimmingProtector;
					lastCaptionSize = g.MeasureString(GetLabelCaption(Colors.Count - 1, getFromValue: false, swatchLabelType), g.ResourceFactory.WrapFont(Font), absoluteRectangle.Size, g.ResourceFactory.CreateTypographicTextFormat());
					lastCaptionSize.Width += TrimmingProtector;
					num2 = Math.Max(firstCaptionSize.Width, lastCaptionSize.Width);
				}
				bool flag2 = !string.IsNullOrEmpty(Title);
				RectangleF layoutRectangle = absoluteRectangle;
				if (flag2)
				{
					float num4 = layoutRectangle.Height = Math.Min(absoluteRectangle.Height, g.MeasureString(Title, g.ResourceFactory.WrapFont(TitleFont), layoutRectangle.Size, g.ResourceFactory.CreateTypographicTextFormat()).Height + (float)TitleSeparatorSize);
					absoluteRectangle.Y += num4;
					absoluteRectangle.Height -= num4;
					titlePosition = layoutRectangle;
				}
				RectangleF colorBarBounds = CalculateMaxColorBarBounds(g, absoluteRectangle, num2, colorsRef.Length, swatchLabelType);
				float num5 = 0f;
				float num6 = 0f;
				if (LabelInterval > 0)
				{
					num5 = GetLabelMaxSize(g, absoluteRectangle.Size, swatchLabelType).Height;
					num6 = TickMarkLength + TickMarkLabelGapSize;
				}
				float val = Math.Max(3f, (absoluteRectangle.Height - num6) / 5f);
				colorBoxSize.Height = Math.Max(val, absoluteRectangle.Height - (float)num * (num6 + num5));
				colorBoxSize.Width = colorBarBounds.Width / (float)colorsRef.Length;
				colorBarBounds.Height = colorBoxSize.Height;
				labelBoxSize.Height = Math.Max(0f, absoluteRectangle.Height - colorBoxSize.Height) / (float)num - num6;
				labelBoxSize.Width = colorBoxSize.Width * (float)LabelInterval * (float)num;
				if (LabelAlignment == LabelAlignment.Top || LabelAlignment == LabelAlignment.Alternate)
				{
					colorBarBounds.Y += labelBoxSize.Height + num6;
				}
				AntiAliasing antiAliasing = g.AntiAliasing;
				try
				{
					g.AntiAliasing = AntiAliasing.None;
					CreateColorBarPath(g, absoluteRectangle, colorBarBounds, colorsRef, swatchLabelType, out IGraphicsPath outlinePath, out IGraphicsPath fillPath);
					IPen pen = g.ResourceFactory.CreatePen(OutlineColor, 1f);
					try
					{
						using (IEnumerator<(IGraphicsPath Path, bool IsClosed)> subpaths = SplitBySubpath(g, fillPath).GetEnumerator())
						{
							int[] array = colorsRef;
							foreach (int colorIndex in array)
							{
								if (!subpaths.MoveNext())
								{
									break;
								}
								if (subpaths.Current.IsClosed)
								{
									using (IBrush brush = CreateColorBoxBrush(g, subpaths.Current.Path.GetBounds(), colorIndex))
									{
										g.FillPath(brush, subpaths.Current.Path);
									}
								}
								subpaths.Current.Path.Dispose();
							}
						}
						g.DrawPath(pen, outlinePath);
					}
					finally
					{
						outlinePath.Dispose();
						fillPath.Dispose();
						pen.Dispose();
					}
				}
				finally
				{
					g.AntiAliasing = antiAliasing;
				}
				if (flag2)
				{
					using (IBrush brush2 = g.ResourceFactory.CreateSolidBrush(TitleColor))
					{
						ITextFormat stringFormat = g.ResourceFactory.CreateTypographicTextFormat();
						stringFormat.Alignment = TitleAlignment;
						stringFormat.LineAlignment = StringAlignment.Near;
						stringFormat.Trimming = StringTrimming.EllipsisCharacter;
						stringFormat.FormatFlags = StringFormatFlags.NoWrap;
						g.DrawString(Title, g.ResourceFactory.WrapFont(TitleFont), brush2, layoutRectangle, stringFormat);
					}
				}
				if (Colors.Count == 0 || LabelInterval == 0)
				{
					return;
				}
				{
					ITextFormat stringFormat2 = g.ResourceFactory.CreateTypographicTextFormat();
					stringFormat2.Alignment = StringAlignment.Center;
					stringFormat2.LineAlignment = StringAlignment.Near;
					stringFormat2.Trimming = StringTrimming.EllipsisCharacter;
					stringFormat2.FormatFlags = StringFormatFlags.NoWrap;
					using (IBrush brush3 = g.ResourceFactory.CreateSolidBrush(LabelColor))
					{
						IChartFont bridgedLabelFont = g.ResourceFactory.WrapFont(Font);
						bool flag3 = LabelAlignment != LabelAlignment.Top;
						if (swatchLabelType == SwatchLabelType.ShowMiddleValue)
						{
							for (int j = 0; j < colorsRef.Length; j++)
							{
								if (!MustPrintLabel(colorsRef, j, isFromValue: true, swatchLabelType))
								{
									continue;
								}
								StringAlignment horizontalAlignemnt;
								RectangleF labelBounds = GetLabelBounds(j, colorsRef, absoluteRectangle, colorBarBounds, labelBoxSize, colorBoxSize, num2, getFromValue: true, swatchLabelType, flag3, firstCaptionSize, lastCaptionSize, out horizontalAlignemnt);
								string labelCaption = GetLabelCaption(j, getFromValue: true, swatchLabelType);
								if (labelBounds.Width > 1f && labelBounds.Height > 1f)
								{
									if (flag3)
									{
										labelBounds.Offset(0f, 1f);
									}
									stringFormat2.Alignment = horizontalAlignemnt;
									g.DrawString(labelCaption, bridgedLabelFont, brush3, labelBounds, stringFormat2);
								}
								flag3 = ((LabelAlignment == LabelAlignment.Alternate) ? (!flag3) : flag3);
							}
							return;
						}
						for (int k = 0; k < colorsRef.Length; k++)
						{
							RectangleF labelBounds2;
							string labelCaption2;
							if (MustPrintLabel(colorsRef, k, isFromValue: true, swatchLabelType))
							{
								labelBounds2 = GetLabelBounds(colorsRef[k], colorsRef, absoluteRectangle, colorBarBounds, labelBoxSize, colorBoxSize, num2, getFromValue: true, swatchLabelType, flag3, firstCaptionSize, lastCaptionSize, out StringAlignment horizontalAlignemnt2);
								labelCaption2 = GetLabelCaption(colorsRef[k], getFromValue: true, swatchLabelType);
								if (labelBounds2.Width > 1f && labelBounds2.Height > 1f)
								{
									if (flag3)
									{
										labelBounds2.Offset(0f, 1f);
									}
									stringFormat2.Alignment = horizontalAlignemnt2;
									g.DrawString(labelCaption2, bridgedLabelFont, brush3, labelBounds2, stringFormat2);
								}
								flag3 = ((LabelAlignment == LabelAlignment.Alternate) ? (!flag3) : flag3);
							}
							if (!MustPrintLabel(colorsRef, k, isFromValue: false, swatchLabelType))
							{
								continue;
							}
							labelBounds2 = GetLabelBounds(colorsRef[k], colorsRef, absoluteRectangle, colorBarBounds, labelBoxSize, colorBoxSize, num2, getFromValue: false, swatchLabelType, flag3, firstCaptionSize, lastCaptionSize, out StringAlignment horizontalAlignemnt3);
							labelCaption2 = GetLabelCaption(colorsRef[k], getFromValue: false, swatchLabelType);
							if (labelBounds2.Width > 1f && labelBounds2.Height > 1f)
							{
								if (flag3)
								{
									labelBounds2.Offset(0f, 1f);
								}
								stringFormat2.Alignment = horizontalAlignemnt3;
								g.DrawString(labelCaption2, bridgedLabelFont, brush3, labelBounds2, stringFormat2);
							}
							flag3 = ((LabelAlignment == LabelAlignment.Alternate) ? (!flag3) : flag3);
						}
					}
				}
			}
			finally
			{
				if (flag)
				{
					Colors.Clear();
				}
			}
		}

		internal override object GetDefaultPropertyValue(string prop, object currentValue)
		{
			if (!(prop == "Dock"))
			{
				if (prop == "Size")
				{
					return new MapSize(null, 350f, 60f);
				}
				return base.GetDefaultPropertyValue(prop, currentValue);
			}
			return PanelDockStyle.Bottom;
		}

		private void CreateColorBarPath(MapGraphics g, RectangleF panelBounds, RectangleF colorBarBounds, int[] colorsRef, SwatchLabelType currentLabelType, out IGraphicsPath outlinePath, out IGraphicsPath fillPath)
		{
			colorBarBounds = Rectangle.Round(colorBarBounds);
			float width = colorBarBounds.Width / (float)colorsRef.Length;
			RectangleF rect = new RectangleF(colorBarBounds.X, colorBarBounds.Y, width, colorBarBounds.Height);
			outlinePath = g.ResourceFactory.CreatePath();
			fillPath = g.ResourceFactory.CreatePath();
			PointF pointF = new PointF(colorBarBounds.Left, colorBarBounds.Bottom);
			PointF pointF2 = new PointF(colorBarBounds.Left, colorBarBounds.Top);
			float num = Math.Min(TickMarkLength + 1, panelBounds.Bottom - pointF.Y);
			float num2 = Math.Min(TickMarkLength, pointF.Y - panelBounds.Top);
			pointF2.Y -= num2;
			if (colorBarBounds.Width <= 0f || colorBarBounds.Width <= 0f)
			{
				return;
			}
			outlinePath.AddRectangle(colorBarBounds);
			for (int i = 0; i < colorsRef.Length; i++)
			{
				fillPath.StartFigure();
				fillPath.AddRectangle(rect);
				outlinePath.StartFigure();
				outlinePath.AddLine(rect.Right, rect.Top, rect.Right, rect.Bottom);
				rect.Offset(rect.Width, 0f);
			}
			bool flag = LabelAlignment != LabelAlignment.Top;
			if (LabelInterval <= 0 || TickMarkLength <= 0)
			{
				return;
			}
			switch (currentLabelType)
			{
			case SwatchLabelType.ShowMiddleValue:
			{
				pointF.X += rect.Width / 2f;
				pointF2.X += rect.Width / 2f;
				for (int k = 0; k < colorsRef.Length; k += LabelInterval)
				{
					if (MustPrintLabel(colorsRef, k, isFromValue: true, currentLabelType))
					{
						PointF empty2 = PointF.Empty;
						float num5 = 0f;
						if (flag)
						{
							empty2 = pointF;
							num5 = num;
						}
						else
						{
							empty2 = pointF2;
							num5 = num2;
						}
						outlinePath.StartFigure();
						outlinePath.AddLine(empty2.X + (float)k * rect.Width, empty2.Y, empty2.X + (float)k * rect.Width, empty2.Y + num5);
						flag = ((LabelAlignment == LabelAlignment.Alternate) ? (!flag) : flag);
					}
				}
				break;
			}
			case SwatchLabelType.ShowBorderValue:
			{
				PointF empty = PointF.Empty;
				float num3 = 0f;
				for (int j = 0; j < colorsRef.Length * 2; j++)
				{
					if (flag)
					{
						empty = pointF;
						num3 = num;
					}
					else
					{
						empty = pointF2;
						num3 = num2;
					}
					if (MustPrintLabel(colorsRef, j / 2, j % 2 == 0, currentLabelType))
					{
						float num4 = 0f;
						if (Colors[colorsRef[j / 2]].NoData)
						{
							num4 = rect.Width / 2f;
						}
						outlinePath.StartFigure();
						outlinePath.AddLine(empty.X + num4 + (float)(j / 2 + j % 2) * rect.Width, empty.Y, empty.X + num4 + (float)(j / 2 + j % 2) * rect.Width, empty.Y + num3);
						flag = ((LabelAlignment == LabelAlignment.Alternate) ? (!flag) : flag);
					}
				}
				break;
			}
			}
		}

		private int[] GetColorsRef(SwatchLabelType currentLabelType)
		{
			if (Colors.Count == 0)
			{
				return new int[0];
			}
			int num = Colors.Count;
			if (currentLabelType == SwatchLabelType.ShowBorderValue)
			{
				for (int i = 1; i < Colors.Count; i++)
				{
					if (!Colors[i - 1].NoData || !Colors[i].NoData)
					{
						if (Colors[i - 1].NoData || Colors[i].NoData)
						{
							num++;
						}
						else if (!Colors[i - 1].HasTextValue && !Colors[i].HasTextValue && Colors[i - 1].ToValue != Colors[i].FromValue)
						{
							num++;
						}
					}
				}
			}
			int[] array = new int[num];
			array[0] = 0;
			int num2 = 1;
			for (int j = 1; j < Colors.Count; j++)
			{
				if (currentLabelType == SwatchLabelType.ShowBorderValue && (!Colors[j - 1].NoData || !Colors[j].NoData))
				{
					if (Colors[j - 1].NoData || Colors[j].NoData)
					{
						array[num2++] = -1;
					}
					else if (!Colors[j - 1].HasTextValue && !Colors[j].HasTextValue && Colors[j - 1].ToValue != Colors[j].FromValue)
					{
						array[num2++] = -1;
					}
				}
				array[num2++] = j;
			}
			return array;
		}

		private RectangleF GetLabelBounds(int colorIndex, int[] colorRef, RectangleF panelBounds, RectangleF colorBarBounds, SizeF labelBoxSize, SizeF colorBoxSize, float longestEndLabelWidth, bool getFromValue, SwatchLabelType curLabelType, bool alignedBottom, SizeF firstCaptionSize, SizeF lastCaptionSize, out StringAlignment horizontalAlignemnt)
		{
			RectangleF rectangleF = new RectangleF(colorBarBounds.Location, labelBoxSize);
			float num = 0f;
			if ((colorIndex == 0 && getFromValue) || (colorIndex == colorRef.Length - 1 && (!getFromValue || curLabelType == SwatchLabelType.ShowMiddleValue)))
			{
				rectangleF.Width = Math.Max(rectangleF.Width, longestEndLabelWidth);
				num = (rectangleF.Width - labelBoxSize.Width) / 2f;
				num = ((!(num > 0f)) ? 0f : (num * (float)((colorIndex != 0) ? 1 : (-1))));
			}
			if (alignedBottom)
			{
				rectangleF.Offset(0f, colorBarBounds.Height + (float)TickMarkLength + (float)TickMarkLabelGapSize);
			}
			else
			{
				rectangleF.Offset(0f, 0f - (labelBoxSize.Height + (float)TickMarkLength + (float)TickMarkLabelGapSize));
			}
			int num2 = Array.IndexOf(colorRef, colorIndex);
			rectangleF.X += (float)num2 * colorBoxSize.Width;
			rectangleF.X += (colorBoxSize.Width - rectangleF.Width) / 2f;
			rectangleF.X += num;
			if (rectangleF.Bottom >= panelBounds.Bottom)
			{
				rectangleF.Height = Math.Max(1f, rectangleF.Height - (rectangleF.Bottom - panelBounds.Bottom));
				rectangleF.Y = panelBounds.Bottom - rectangleF.Height;
			}
			if (!Colors[colorIndex].NoData && curLabelType == SwatchLabelType.ShowBorderValue)
			{
				rectangleF.Offset(getFromValue ? ((0f - colorBoxSize.Width) / 2f) : (colorBoxSize.Width / 2f), 0f);
			}
			float num3 = panelBounds.Left - rectangleF.Left;
			bool flag = false;
			bool flag2 = false;
			if (num3 > 0f)
			{
				RectangleF rectangleF2 = rectangleF;
				rectangleF2.Inflate(0f - num3, 0f);
				if (rectangleF2.Width > firstCaptionSize.Width)
				{
					rectangleF = rectangleF2;
				}
				else
				{
					rectangleF.X += num3;
					rectangleF.Width -= num3;
					flag = true;
				}
			}
			float num4 = rectangleF.Right - panelBounds.Right;
			if (num4 > 0f)
			{
				RectangleF rectangleF3 = rectangleF;
				rectangleF3.Inflate(0f - num4, 0f);
				if (rectangleF3.Width > lastCaptionSize.Width)
				{
					rectangleF = rectangleF3;
				}
				else
				{
					rectangleF.Width -= num4;
					flag2 = true;
				}
			}
			if (flag && flag2)
			{
				horizontalAlignemnt = StringAlignment.Center;
			}
			else if (flag)
			{
				horizontalAlignemnt = StringAlignment.Near;
			}
			else if (flag2)
			{
				horizontalAlignemnt = StringAlignment.Far;
			}
			else
			{
				horizontalAlignemnt = StringAlignment.Center;
			}
			rectangleF.Height += 1f;
			return Rectangle.Round(rectangleF);
		}

		private RectangleF CalculateMaxColorBarBounds(MapGraphics g, RectangleF bounds, float endCaptionWidth, int colorsNumber, SwatchLabelType currentLabelType)
		{
			if (LabelInterval == 0)
			{
				return bounds;
			}
			_ = LabelAlignment;
			_ = 2;
			float num = endCaptionWidth;
			if (currentLabelType == SwatchLabelType.ShowMiddleValue)
			{
				num = ((Colors.Count != 1) ? (((float)Colors.Count * endCaptionWidth - bounds.Width) / (float)(Colors.Count - 1)) : (num - bounds.Width / (float)colorsNumber));
				num = ((num < 0f) ? 0f : num);
			}
			bounds.Inflate((0f - num) / 2f, 0f);
			return bounds;
		}

		private string GetLabelCaption(int swatchColorIndex, bool getFromValue, SwatchLabelType currentLabelType)
		{
			SwatchColor swatchColor = Colors[swatchColorIndex];
			if (swatchColor.NoData)
			{
				return NoDataText;
			}
			string result = "";
			if (currentLabelType == SwatchLabelType.ShowBorderValue)
			{
				if (!swatchColor.HasTextValue)
				{
					double num = (!getFromValue) ? swatchColor.ToValue : swatchColor.FromValue;
					result = ((GetMapCore().MapControl.FormatNumberHandler == null) ? num.ToString(NumericLabelFormat, CultureInfo.CurrentCulture) : GetMapCore().MapControl.FormatNumberHandler(GetMapCore().MapControl, num, NumericLabelFormat));
				}
			}
			else
			{
				if (swatchColor.HasTextValue)
				{
					return swatchColor.TextValue;
				}
				double num2 = (swatchColor.FromValue + swatchColor.ToValue) / 2.0;
				result = ((GetMapCore().MapControl.FormatNumberHandler == null) ? num2.ToString(NumericLabelFormat, CultureInfo.CurrentCulture) : GetMapCore().MapControl.FormatNumberHandler(GetMapCore().MapControl, num2, NumericLabelFormat));
			}
			return result;
		}

		private SizeF GetLabelMaxSize(MapGraphics g, SizeF layoutArea, SwatchLabelType currentLabelType)
		{
			float num = 0f;
			float num2 = 0f;
			IChartFont bridgedFont = g.ResourceFactory.WrapFont(Font);
			ITextFormat typographicFormat = g.ResourceFactory.CreateTypographicTextFormat();
			for (int i = 0; i < Colors.Count; i++)
			{
				string labelCaption = GetLabelCaption(i, getFromValue: true, currentLabelType);
				SizeF sizeF = g.MeasureString(labelCaption, bridgedFont, layoutArea, typographicFormat);
				num = Math.Max(num, sizeF.Height);
				num2 = Math.Max(num2, sizeF.Width + TrimmingProtector);
				if (currentLabelType == SwatchLabelType.ShowBorderValue)
				{
					labelCaption = GetLabelCaption(i, getFromValue: false, currentLabelType);
					sizeF = g.MeasureString(labelCaption, bridgedFont, layoutArea, typographicFormat);
					num = Math.Max(num, sizeF.Height);
					num2 = Math.Max(num2, sizeF.Width + TrimmingProtector);
				}
			}
			return System.Drawing.Size.Round(new SizeF(num2, num));
		}

		private bool MustPrintLabel(int[] colorsRef, int colorRefIndex, bool isFromValue, SwatchLabelType currentLabelType)
		{
			if (LabelInterval <= 0)
			{
				return false;
			}
			int num = colorRefIndex + ((currentLabelType == SwatchLabelType.ShowBorderValue && !isFromValue) ? 1 : 0);
			if (colorRefIndex == 0 && isFromValue)
			{
				if (ShowEndLabels)
				{
					return true;
				}
				return Colors[colorsRef[colorRefIndex]].NoData;
			}
			if (colorRefIndex == colorsRef.Length - 1 && !ShowEndLabels && (currentLabelType == SwatchLabelType.ShowMiddleValue || !isFromValue))
			{
				return false;
			}
			if (currentLabelType == SwatchLabelType.ShowMiddleValue && num % LabelInterval == 0)
			{
				return true;
			}
			if (colorsRef[colorRefIndex] == -1 || (!Colors[colorsRef[colorRefIndex]].NoData && Colors[colorsRef[colorRefIndex]].HasTextValue) || num % LabelInterval != 0)
			{
				return false;
			}
			if (!isFromValue)
			{
				if (!Colors[colorsRef[colorRefIndex]].NoData)
				{
					return true;
				}
				return false;
			}
			if (MustPrintLabel(colorsRef, colorRefIndex - 1, isFromValue: false, currentLabelType))
			{
				return Colors[colorsRef[colorRefIndex]].NoData;
			}
			return true;
		}

		private IBrush CreateColorBoxBrush(MapGraphics g, RectangleF colorBoxBoundsAbs, int colorIndex)
		{
			if (colorIndex < 0)
			{
				return g.ResourceFactory.CreateSolidBrush(RangeGapColor);
			}
			SwatchColor swatchColor = Colors[colorIndex];
			Color color = swatchColor.Color;
			Color secondaryColor = swatchColor.SecondaryColor;
			GradientType gradientType = swatchColor.GradientType;
			MapHatchStyle hatchStyle = swatchColor.HatchStyle;
			if (hatchStyle != 0)
			{
				return g.ResourceFactory.WrapBrush(MapGraphics.GetHatchBrush(hatchStyle, color, secondaryColor));
			}
			if (gradientType != 0)
			{
				return g.ResourceFactory.WrapBrush(g.GetGradientBrush(colorBoxBoundsAbs, color, secondaryColor, gradientType));
			}
			return g.ResourceFactory.CreateSolidBrush(color);
		}

		private void CalculateFontDependentData(MapGraphics g, SizeF layoutSize)
		{
			IChartFont bridgedFont = g.ResourceFactory.WrapFont(Font);
			ITextFormat typographicFormat = g.ResourceFactory.CreateTypographicTextFormat();
			SizeF sizeF = g.MeasureString("M", bridgedFont, layoutSize, typographicFormat);
			SizeF sizeF2 = g.MeasureString("MM", bridgedFont, layoutSize, typographicFormat);
			float height = sizeF.Height;
			_ = sizeF2.Width;
			_ = sizeF.Width;
			PanelPadding = (int)Math.Round(height);
			TitleSeparatorSize = (int)Math.Round((double)height * 0.4);
			TickMarkLabelGapSize = (int)Math.Round((double)height * 0.1);
			TrimmingProtector = g.MeasureString("..", bridgedFont, layoutSize, typographicFormat).Width;
		}

		private void PopulateDummyData()
		{
			if (Colors.Count == 0)
			{
				Color[] array = new ColorGenerator().GenerateColors(MapColorPalette.Light, 5);
				for (int i = 0; i < array.Length; i++)
				{
					SwatchColor swatchColor = new SwatchColor("", (i + 1) * 100, (i + 2) * 100);
					swatchColor.Color = array[i];
					Colors.Add(swatchColor);
				}
			}
		}

		internal override SizeF GetOptimalSize(MapGraphics g, SizeF maxSizeAbs)
		{
			if (!IsVisible())
			{
				return SizeF.Empty;
			}
			bool flag = false;
			try
			{
				if (Colors.Count == 0 && Common != null && Common.MapCore.IsDesignMode())
				{
					PopulateDummyData();
					flag = true;
				}
				CalculateFontDependentData(g, maxSizeAbs);
				SwatchLabelType swatchLabelType = GetLabelType();
				float num = 2 * PanelPadding;
				float val = 0f;
				SizeF sizeF = default(SizeF);
				SizeF sizeF2 = default(SizeF);
				float num2 = 0f;
				if (LabelInterval > 0 && ShowEndLabels)
				{
					sizeF = g.MeasureString(GetLabelCaption(0, getFromValue: true, swatchLabelType), g.ResourceFactory.WrapFont(Font), maxSizeAbs, g.ResourceFactory.CreateTypographicTextFormat());
					sizeF.Width += TrimmingProtector;
					sizeF2 = g.MeasureString(GetLabelCaption(Colors.Count - 1, getFromValue: false, swatchLabelType), g.ResourceFactory.WrapFont(Font), maxSizeAbs, g.ResourceFactory.CreateTypographicTextFormat());
					sizeF2.Width += TrimmingProtector;
					num2 = (float)Math.Round(Math.Max(sizeF.Width, sizeF2.Width));
				}
				int num3 = (LabelAlignment != LabelAlignment.Alternate) ? 1 : 2;
				int[] colorsRef = GetColorsRef(swatchLabelType);
				SizeF labelMaxSize = GetLabelMaxSize(g, maxSizeAbs, swatchLabelType);
				float height = labelMaxSize.Height;
				float num4 = Math.Max(2f * height, labelMaxSize.Width / (float)(num3 * LabelInterval));
				num += height + (float)num3 * (labelMaxSize.Height + (float)TickMarkLength + (float)TickMarkLabelGapSize);
				if (!string.IsNullOrEmpty(Title))
				{
					SizeF sizeF3 = g.MeasureString(Title, g.ResourceFactory.WrapFont(TitleFont), maxSizeAbs, g.ResourceFactory.CreateTypographicTextFormat());
					num += sizeF3.Height + (float)TitleSeparatorSize;
					val = sizeF3.Width + TrimmingProtector;
				}
				val = Math.Max(val, num4 * (float)colorsRef.Length);
				float num5 = num2;
				if (swatchLabelType == SwatchLabelType.ShowMiddleValue)
				{
					num5 -= num4;
					num5 = ((num5 < 0f) ? 0f : num5);
				}
				val += num5;
				val += (float)(2 * PanelPadding + 2 * BorderWidth);
				num += (float)(2 * BorderWidth);
				SizeF result = new SizeF(Math.Min(maxSizeAbs.Width, val), Math.Min(maxSizeAbs.Height, num));
				result.Width = Math.Max(2f, result.Width);
				result.Height = Math.Max(2f, result.Height);
				return result;
			}
			finally
			{
				if (flag)
				{
					Colors.Clear();
				}
			}
		}

		// Splits a path into its individual subpaths (each figure delimited by a PathPointTypeStart
		// point), mirroring GraphicsPathIterator.NextSubpath's behavior without depending on the
		// GDI+-only GraphicsPathIterator type. Distinct from the PathMarker-based SplitAtMarkers helper
		// used elsewhere (HotRegionsList.cs/MapAreasCollection.cs) - this splits by figure boundary, not
		// by an explicit marker bit, since CreateColorBarPath builds fillPath from StartFigure/AddRectangle
		// per swatch rather than SetMarkers.
		private static IEnumerable<(IGraphicsPath Path, bool IsClosed)> SplitBySubpath(MapGraphics graph, IGraphicsPath path)
		{
			const byte PathTypeMask = 0x07;
			const byte PathTypeStart = 0x00;
			const byte CloseSubpath = 0x80;
			PointF[] points = path.PathPoints;
			byte[] types = path.PathTypes;
			int start = 0;
			for (int i = 1; i <= points.Length; i++)
			{
				bool atNextStart = i < points.Length && (types[i] & PathTypeMask) == PathTypeStart;
				if (atNextStart || i == points.Length)
				{
					bool isClosed = (types[i - 1] & CloseSubpath) != 0;
					yield return (graph.ResourceFactory.CreatePath(CopySegment(points, start, i), CopySegment(types, start, i)), isClosed);
					start = i;
				}
			}
		}

		private static T[] CopySegment<T>(T[] source, int start, int endExclusive)
		{
			T[] array = new T[endExclusive - start];
			Array.Copy(source, start, array, 0, array.Length);
			return array;
		}
	}
}
