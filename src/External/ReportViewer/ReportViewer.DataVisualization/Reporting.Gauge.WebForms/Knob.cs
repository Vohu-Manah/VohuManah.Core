using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using Reporting.Rendering;

namespace Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(KnobConverter))]
	internal class Knob : PointerBase, ISelectable
	{
		private bool capVisible = true;

		private bool capReflection = true;

		private float capWidth = 60f;

		private string capImage = "";

		private Color capImageTransColor = Color.Empty;

		private Color capImageHueColor = Color.Empty;

		private Point capImageOrigin = Point.Empty;

		private KnobStyle style;

		private Color capFillColor = Color.Gainsboro;

		private GradientType capFillGradientType = GradientType.DiagonalLeft;

		private Color capFillGradientEndColor = Color.Gray;

		private GaugeHatchStyle capFillHatchStyle;

		private float markerLength = 15f;

		private float markerWidth = 10f;

		private float markerPosition = 36f;

		private Color markerFillColor = Color.DarkGray;

		private GradientType markerFillGradientType = GradientType.VerticalCenter;

		private Color markerFillGradientEndColor = Color.White;

		private GaugeHatchStyle markerFillHatchStyle;

		private float capShadowOffset = 2f;

		private bool rotateGradient;

		private bool capRotateGradient;

		private bool markerRotateGradient = true;

		private bool selected;

		private IGraphicsPath[] hotRegions = new IGraphicsPath[2];

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeKnob_Name")]
		public override string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryMisc")]
		[SRDescription("DescriptionAttributeKnob_ScaleName")]
		[DefaultValue("Default")]
		[TypeConverter(typeof(ScaleSourceConverter))]
		public override string ScaleName
		{
			get
			{
				return base.ScaleName;
			}
			set
			{
				base.ScaleName = value;
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeKnob_Image")]
		[DefaultValue("")]
		public override string Image
		{
			get
			{
				return base.Image;
			}
			set
			{
				base.Image = value;
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeKnob_ImageTransColor")]
		[DefaultValue(typeof(Color), "")]
		public override Color ImageTransColor
		{
			get
			{
				return base.ImageTransColor;
			}
			set
			{
				base.ImageTransColor = value;
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeKnob_ImageHueColor")]
		[DefaultValue(typeof(Color), "")]
		public override Color ImageHueColor
		{
			get
			{
				return base.ImageHueColor;
			}
			set
			{
				base.ImageHueColor = value;
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeKnob_ImageOrigin")]
		[TypeConverter(typeof(EmptyPointConverter))]
		[DefaultValue(typeof(Point), "0, 0")]
		public override Point ImageOrigin
		{
			get
			{
				return base.ImageOrigin;
			}
			set
			{
				base.ImageOrigin = value;
			}
		}

		[SRCategory("CategoryData")]
		[SRDescription("DescriptionAttributeKnob_Value")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[DefaultValue(double.NaN)]
		[TypeConverter(typeof(DoubleNanValueConverter))]
		public override double Value
		{
			get
			{
				return base.Value;
			}
			set
			{
				base.Value = value;
			}
		}

		[SRCategory("CategoryData")]
		[SRDescription("DescriptionAttributeKnob_ValueSource")]
		[TypeConverter(typeof(ValueSourceConverter))]
		[RefreshProperties(RefreshProperties.Repaint)]
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		public override string ValueSource
		{
			get
			{
				return base.ValueSource;
			}
			set
			{
				base.ValueSource = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeKnob_SnappingEnabled")]
		[DefaultValue(false)]
		public override bool SnappingEnabled
		{
			get
			{
				return base.SnappingEnabled;
			}
			set
			{
				base.SnappingEnabled = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeKnob_DampeningEnabled")]
		[DefaultValue(false)]
		public override bool DampeningEnabled
		{
			get
			{
				return base.DampeningEnabled;
			}
			set
			{
				base.DampeningEnabled = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeKnob_ToolTip")]
		[Localizable(true)]
		[DefaultValue("")]
		public override string ToolTip
		{
			get
			{
				return base.ToolTip;
			}
			set
			{
				base.ToolTip = value;
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeKnob_Interactive")]
		[DefaultValue(true)]
		public override bool Interactive
		{
			get
			{
				return base.Interactive;
			}
			set
			{
				base.Interactive = value;
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeKnob_Visible")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				base.Visible = value;
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeKnob_FillColor")]
		[DefaultValue(typeof(Color), "Gainsboro")]
		public override Color FillColor
		{
			get
			{
				return base.FillColor;
			}
			set
			{
				base.FillColor = value;
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeKnob_FillGradientEndColor")]
		[DefaultValue(typeof(Color), "Gray")]
		public override Color FillGradientEndColor
		{
			get
			{
				return base.FillGradientEndColor;
			}
			set
			{
				base.FillGradientEndColor = value;
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeKnob_FillHatchStyle")]
		[DefaultValue(GaugeHatchStyle.None)]
		public override GaugeHatchStyle FillHatchStyle
		{
			get
			{
				return base.FillHatchStyle;
			}
			set
			{
				base.FillHatchStyle = value;
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeKnob_FillGradientType")]
		[DefaultValue(GradientType.DiagonalLeft)]
		public override GradientType FillGradientType
		{
			get
			{
				return base.FillGradientType;
			}
			set
			{
				base.FillGradientType = value;
			}
		}

		[SRCategory("CategoryKnobCap")]
		[SRDescription("DescriptionAttributeKnob_CapVisible")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
		public bool CapVisible
		{
			get
			{
				return capVisible;
			}
			set
			{
				capVisible = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryKnobCap")]
		[SRDescription("DescriptionAttributeKnob_CapReflection")]
		[DefaultValue(true)]
		public bool CapReflection
		{
			get
			{
				return capReflection;
			}
			set
			{
				capReflection = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryKnobCap")]
		[SRDescription("DescriptionAttributeKnob_CapWidth")]
		[ValidateBound(0.0, 100.0)]
		[DefaultValue(60f)]
		public float CapWidth
		{
			get
			{
				return capWidth;
			}
			set
			{
				if (value < 0f || value > 100f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
				}
				capWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeKnob_CapImage")]
		[DefaultValue("")]
		public string CapImage
		{
			get
			{
				return capImage;
			}
			set
			{
				capImage = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeKnob_CapImageTransColor")]
		[DefaultValue(typeof(Color), "")]
		public Color CapImageTransColor
		{
			get
			{
				return capImageTransColor;
			}
			set
			{
				capImageTransColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeCapImageHueColor")]
		[DefaultValue(typeof(Color), "")]
		public Color CapImageHueColor
		{
			get
			{
				return capImageHueColor;
			}
			set
			{
				capImageHueColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryImage")]
		[SRDescription("DescriptionAttributeKnob_CapImageOrigin")]
		[TypeConverter(typeof(EmptyPointConverter))]
		[DefaultValue(typeof(Point), "0, 0")]
		public Point CapImageOrigin
		{
			get
			{
				return capImageOrigin;
			}
			set
			{
				capImageOrigin = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeKnob_Width")]
		[ValidateBound(0.0, 200.0)]
		[DefaultValue(80f)]
		public override float Width
		{
			get
			{
				return base.Width;
			}
			set
			{
				if (value < 0f || value > 1000f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 1000));
				}
				base.Width = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryMarker")]
		[SRDescription("DescriptionAttributeKnob_Style")]
		[DefaultValue(MarkerStyle.Wedge)]
		public override MarkerStyle MarkerStyle
		{
			get
			{
				return base.MarkerStyle;
			}
			set
			{
				base.MarkerStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeKnob_Style")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(KnobStyle.Style1)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Obsolete("This property is obsolete in Dundas Gauge 2.0. KnobStyle is supposed to be used instead.")]
		public KnobStyle Style
		{
			get
			{
				return style;
			}
			set
			{
				style = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeKnob_Style")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(KnobStyle.Style1)]
		public KnobStyle KnobStyle
		{
			get
			{
				return style;
			}
			set
			{
				style = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryKnobCap")]
		[SRDescription("DescriptionAttributeKnob_CapFillColor")]
		[DefaultValue(typeof(Color), "Gainsboro")]
		public Color CapFillColor
		{
			get
			{
				return capFillColor;
			}
			set
			{
				capFillColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryKnobCap")]
		[SRDescription("DescriptionAttributeKnob_MarkerFillGradientType")]
		[DefaultValue(GradientType.DiagonalLeft)]
		public GradientType CapFillGradientType
		{
			get
			{
				return capFillGradientType;
			}
			set
			{
				capFillGradientType = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryKnobCap")]
		[SRDescription("DescriptionAttributeKnob_CapFillGradientEndColor")]
		[DefaultValue(typeof(Color), "Gray")]
		public Color CapFillGradientEndColor
		{
			get
			{
				return capFillGradientEndColor;
			}
			set
			{
				capFillGradientEndColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryKnobCap")]
		[SRDescription("DescriptionAttributeKnob_CapFillHatchStyle")]
		[DefaultValue(GaugeHatchStyle.None)]
		public GaugeHatchStyle CapFillHatchStyle
		{
			get
			{
				return capFillHatchStyle;
			}
			set
			{
				capFillHatchStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryMarker")]
		[SRDescription("DescriptionAttributeKnob_MarkerLength")]
		[ValidateBound(0.0, 50.0)]
		[DefaultValue(15f)]
		public override float MarkerLength
		{
			get
			{
				return markerLength;
			}
			set
			{
				if (value < 0f || value > 1000f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 1000));
				}
				markerLength = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryMarker")]
		[SRDescription("DescriptionAttributeKnob_MarkerWidth")]
		[ValidateBound(0.0, 50.0)]
		[DefaultValue(10f)]
		public float MarkerWidth
		{
			get
			{
				return markerWidth;
			}
			set
			{
				if (value < 0f || value > 1000f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 1000));
				}
				markerWidth = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryMarker")]
		[SRDescription("DescriptionAttributeKnob_MarkerPosition")]
		[ValidateBound(0.0, 100.0)]
		[DefaultValue(36f)]
		public float MarkerPosition
		{
			get
			{
				return markerPosition;
			}
			set
			{
				if (value < 0f || value > 100f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
				}
				markerPosition = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryMarker")]
		[SRDescription("DescriptionAttributeKnob_MarkerFillColor")]
		[DefaultValue(typeof(Color), "DarkGray")]
		public Color MarkerFillColor
		{
			get
			{
				return markerFillColor;
			}
			set
			{
				markerFillColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryMarker")]
		[SRDescription("DescriptionAttributeKnob_MarkerFillGradientType")]
		[DefaultValue(GradientType.VerticalCenter)]
		public GradientType MarkerFillGradientType
		{
			get
			{
				return markerFillGradientType;
			}
			set
			{
				markerFillGradientType = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryMarker")]
		[SRDescription("DescriptionAttributeKnob_MarkerFillGradientEndColor")]
		[DefaultValue(typeof(Color), "White")]
		public Color MarkerFillGradientEndColor
		{
			get
			{
				return markerFillGradientEndColor;
			}
			set
			{
				markerFillGradientEndColor = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryMarker")]
		[SRDescription("DescriptionAttributeKnob_MarkerFillHatchStyle")]
		[DefaultValue(GaugeHatchStyle.None)]
		public GaugeHatchStyle MarkerFillHatchStyle
		{
			get
			{
				return markerFillHatchStyle;
			}
			set
			{
				markerFillHatchStyle = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryKnobCap")]
		[SRDescription("DescriptionAttributeKnob_CapShadowOffset")]
		[ValidateBound(-5.0, 5.0)]
		[DefaultValue(2f)]
		public float CapShadowOffset
		{
			get
			{
				return capShadowOffset;
			}
			set
			{
				if (value < -100f || value > 100f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
				}
				capShadowOffset = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeKnob_RotateGradient")]
		[DefaultValue(false)]
		public bool RotateGradient
		{
			get
			{
				return rotateGradient;
			}
			set
			{
				rotateGradient = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryKnobCap")]
		[SRDescription("DescriptionAttributeKnob_CapRotateGradient")]
		[DefaultValue(false)]
		public bool CapRotateGradient
		{
			get
			{
				return capRotateGradient;
			}
			set
			{
				capRotateGradient = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryMarker")]
		[SRDescription("DescriptionAttributeKnob_MarkerRotateGradient")]
		[DefaultValue(true)]
		public bool MarkerRotateGradient
		{
			get
			{
				return markerRotateGradient;
			}
			set
			{
				markerRotateGradient = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryBehavior")]
		[SRDescription("DescriptionAttributeKnob_Cursor")]
		[DefaultValue(GaugeCursor.Default)]
		public override GaugeCursor Cursor
		{
			get
			{
				return base.Cursor;
			}
			set
			{
				base.Cursor = value;
			}
		}

		[SRCategory("CategoryAppearance")]
		[SRDescription("DescriptionAttributeKnob_Selected")]
		[DefaultValue(false)]
		public bool Selected
		{
			get
			{
				return selected;
			}
			set
			{
				selected = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override float DistanceFromScale
		{
			get
			{
				return base.DistanceFromScale;
			}
			set
			{
				base.DistanceFromScale = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override BarStart BarStart
		{
			get
			{
				return base.BarStart;
			}
			set
			{
				base.BarStart = value;
			}
		}

		public Knob()
			: base(MarkerStyle.Wedge, 15f, 80f, GradientType.DiagonalLeft, Color.Gainsboro, Color.Gray, interactive: true)
		{
		}

		protected bool ShouldSerializeStyle()
		{
			return false;
		}

		private static Matrix3x2 ToMatrix3x2(Matrix nativeMatrix)
		{
			float[] elements = nativeMatrix.Elements;
			return new Matrix3x2(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]);
		}

		internal KnobStyleAttrib GetKnobStyleAttrib(GaugeGraphics g, PointF pointOrigin, float angle)
		{
			KnobStyleAttrib knobStyleAttrib = new KnobStyleAttrib();
			if (Image != "" && CapImage != "")
			{
				return knobStyleAttrib;
			}
			float absoluteDimension = g.GetAbsoluteDimension(Width);
			float num = CapWidth / 100f * absoluteDimension;
			knobStyleAttrib.paths = new IGraphicsPath[6];
			knobStyleAttrib.brushes = new IBrush[6];
			if (Image == "")
			{
				GraphicsPath knobPath = GetKnobPath(g, absoluteDimension, absoluteDimension * 0.5f);
				knobStyleAttrib.paths[0] = (knobPath != null) ? g.ResourceFactory.WrapPath(knobPath) : null;
			}
			else
			{
				knobStyleAttrib.paths[0] = null;
			}
			if (CapVisible && CapImage == "")
			{
				if (CapShadowOffset != 0f)
				{
					IGraphicsPath path = g.ResourceFactory.CreatePath();
					path.AddEllipse(0f - num + CapShadowOffset, 0f - num + CapShadowOffset, num * 2f, num * 2f);
					using (Matrix matrix = new Matrix())
					{
						matrix.Translate(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
						path.Transform(ToMatrix3x2(matrix));
					}
					knobStyleAttrib.paths[1] = path;
				}
				IGraphicsPath path2 = g.ResourceFactory.CreatePath();
				path2.AddEllipse(0f - num, 0f - num, num * 2f, num * 2f);
				knobStyleAttrib.paths[2] = path2;
			}
			else
			{
				if (CapShadowOffset == 0f)
				{
					knobStyleAttrib.paths[1] = null;
				}
				knobStyleAttrib.paths[2] = null;
			}
			float y = MarkerPosition / 100f * absoluteDimension * 2f;
			float num2 = MarkerWidth / 100f * absoluteDimension * 2f;
			float markerHeight = MarkerLength / 100f * absoluteDimension * 2f;
			PointF point = new PointF(0f, y);
			IGraphicsPath path3 = g.ResourceFactory.WrapPath(g.CreateMarker(point, num2, markerHeight, MarkerStyle));
			using (Matrix matrix2 = new Matrix())
			{
				matrix2.RotateAt(180f, point, MatrixOrder.Append);
				if (!MarkerRotateGradient)
				{
					matrix2.Rotate(angle, MatrixOrder.Append);
					matrix2.Translate(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				}
				path3.Transform(ToMatrix3x2(matrix2));
			}
			knobStyleAttrib.paths[3] = path3;
			if (Image == "" && knobStyleAttrib.paths[0] != null)
			{
				float angle2 = RotateGradient ? angle : 0f;
				knobStyleAttrib.brushes[0] = GetFillBrushResource(g, knobStyleAttrib.paths[0], pointOrigin, angle2, FillColor, FillGradientType, FillGradientEndColor, FillHatchStyle);
			}
			else
			{
				knobStyleAttrib.brushes[0] = null;
			}
			if (CapVisible && CapImage == "")
			{
				if (CapShadowOffset != 0f)
				{
					knobStyleAttrib.brushes[1] = g.GetShadowBrushResource();
				}
				float angle3 = CapRotateGradient ? angle : 0f;
				knobStyleAttrib.brushes[2] = GetFillBrushResource(g, knobStyleAttrib.paths[2], pointOrigin, angle3, CapFillColor, CapFillGradientType, CapFillGradientEndColor, CapFillHatchStyle);
			}
			else
			{
				if (CapShadowOffset == 0f)
				{
					knobStyleAttrib.brushes[1] = null;
				}
				knobStyleAttrib.brushes[2] = null;
			}
			float angle4 = MarkerRotateGradient ? angle : 0f;
			PointF pointOrigin2 = pointOrigin;
			if (!MarkerRotateGradient)
			{
				pointOrigin2 = new PointF(0f, 0f);
			}
			knobStyleAttrib.brushes[3] = g.GetMarkerBrushResource(knobStyleAttrib.paths[3], MarkerStyle, pointOrigin2, angle4, MarkerFillColor, MarkerFillGradientType, MarkerFillGradientEndColor, MarkerFillHatchStyle);
			if (CapVisible && CapReflection && CapImage == "")
			{
				g.GetCircularEdgeReflectionResource(knobStyleAttrib.paths[2].GetBounds(), 135f, 200, pointOrigin, out knobStyleAttrib.paths[4], out knobStyleAttrib.brushes[4]);
				g.GetCircularEdgeReflectionResource(knobStyleAttrib.paths[2].GetBounds(), 315f, 128, pointOrigin, out knobStyleAttrib.paths[5], out knobStyleAttrib.brushes[5]);
			}
			else
			{
				knobStyleAttrib.paths[4] = null;
				knobStyleAttrib.paths[5] = null;
				knobStyleAttrib.brushes[4] = null;
				knobStyleAttrib.brushes[5] = null;
			}
			using (Matrix matrix3 = new Matrix())
			{
				matrix3.Rotate(angle, MatrixOrder.Append);
				matrix3.Translate(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				Matrix3x2 matrix3x2 = ToMatrix3x2(matrix3);
				if (knobStyleAttrib.paths[0] != null)
				{
					knobStyleAttrib.paths[0].Transform(matrix3x2);
				}
				if (knobStyleAttrib.paths[2] != null)
				{
					knobStyleAttrib.paths[2].Transform(matrix3x2);
				}
				if (knobStyleAttrib.paths[3] != null && MarkerRotateGradient)
				{
					knobStyleAttrib.paths[3].Transform(matrix3x2);
				}
				return knobStyleAttrib;
			}
		}

		private GraphicsPath GetKnobPath(GaugeGraphics g, float knobRadius, float capRadius)
		{
			if ((double)knobRadius < 0.0001)
			{
				return null;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			if (KnobStyle == KnobStyle.Style1)
			{
				float num = knobRadius * 0.95f;
				bool flag = false;
				for (int i = 15; i < 360; i += 30)
				{
					if (flag)
					{
						graphicsPath.AddArc(0f - knobRadius, 0f - knobRadius, knobRadius * 2f, knobRadius * 2f, i, 30f);
					}
					else
					{
						graphicsPath.AddArc(0f - num, 0f - num, num * 2f, num * 2f, i, 30f);
					}
					flag = !flag;
				}
			}
			else if (KnobStyle == KnobStyle.Style2)
			{
				for (int j = 15; j < 360; j += 30)
				{
					using (GraphicsPath graphicsPath2 = new GraphicsPath())
					{
						float num2 = knobRadius - capRadius;
						float num3 = capRadius + num2;
						graphicsPath2.AddArc((0f - num2) / 6f, num3 - num2 / 6f, num2 / 3f, num2 / 3f, 0f, -180f);
						using (Matrix matrix = new Matrix())
						{
							matrix.Rotate(j);
							graphicsPath2.Transform(matrix);
						}
						graphicsPath.AddPath(graphicsPath2, connect: true);
					}
				}
			}
			else if (KnobStyle == KnobStyle.Style3)
			{
				graphicsPath.AddEllipse(0f - knobRadius, 0f - knobRadius, knobRadius * 2f, knobRadius * 2f);
			}
			graphicsPath.CloseFigure();
			return graphicsPath;
		}

		/// <summary>
		/// Interface-typed replacement for the formerly-concrete <c>GetFillBrush</c> (removed — this is now
		/// the sole/real producer, dead code eliminated same as <c>GetSpecialCapBrush</c> earlier). Unblocked
		/// by the brush-transform gap-fill
		/// (<see cref="ILinearGradientBrush.RotateTransform"/>/<see cref="ILinearGradientBrush.TranslateTransform"/>/
		/// <see cref="ILinearGradientBrush.SetRotationTransform"/> and the <c>IPathGradientBrush</c> equivalents).
		/// Now the real producer for <c>KnobStyleAttrib.brushes[]</c> (<see cref="IBrush"/> array) after the
		/// atomic retyping pass — see tasks/gauge-gdi-type-abstraction.md Milestone B2/B3.
		/// </summary>
		private IBrush GetFillBrushResource(GaugeGraphics g, IGraphicsPath path, PointF pointOrigin, float angle, Color fillColor, GradientType fillGradientType, Color fillGradientEndColor, GaugeHatchStyle fillHatchStyle)
		{
			IBrush brush;
			if (fillHatchStyle != 0)
			{
				return g.GetHatchBrushResource(fillHatchStyle, fillColor, fillGradientEndColor);
			}
			if (fillGradientType != 0)
			{
				RectangleF bounds = path.GetBounds();
				switch (fillGradientType)
				{
				case GradientType.DiagonalLeft:
				{
					ILinearGradientBrush linearGradientBrush2 = (ILinearGradientBrush)g.GetGradientBrushResource(bounds, fillColor, fillGradientEndColor, GradientType.LeftRight);
					linearGradientBrush2.SetRotationTransform(45f, new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f));
					brush = linearGradientBrush2;
					break;
				}
				case GradientType.DiagonalRight:
				{
					ILinearGradientBrush linearGradientBrush = (ILinearGradientBrush)g.GetGradientBrushResource(bounds, fillColor, fillGradientEndColor, GradientType.TopBottom);
					linearGradientBrush.SetRotationTransform(135f, new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f));
					brush = linearGradientBrush;
					break;
				}
				case GradientType.Center:
				{
					bounds.Inflate(1f, 1f);
					using GraphicsPath graphicsPath = new GraphicsPath();
					graphicsPath.AddArc(bounds, 0f, 360f);
					IPathGradientBrush pathGradientBrush = g.ResourceFactory.CreatePathGradientBrush(g.ResourceFactory.WrapPath(graphicsPath));
					pathGradientBrush.CenterColor = fillColor;
					pathGradientBrush.CenterPoint = new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
					pathGradientBrush.SurroundColors = new Color[1] { fillGradientEndColor };
					brush = pathGradientBrush;
					break;
				}
				default:
					brush = g.GetGradientBrushResource(path.GetBounds(), fillColor, fillGradientEndColor, fillGradientType);
					break;
				}
				if (brush is ILinearGradientBrush linearGradientBrush3)
				{
					linearGradientBrush3.RotateTransform(angle, MatrixOrder.Append);
					linearGradientBrush3.TranslateTransform(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				}
				else if (brush is IPathGradientBrush pathGradientBrush2)
				{
					pathGradientBrush2.RotateTransform(angle, MatrixOrder.Append);
					pathGradientBrush2.TranslateTransform(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				}
			}
			else
			{
				brush = g.ResourceFactory.CreateSolidBrush(fillColor);
			}
			return brush;
		}

		internal override void Render(GaugeGraphics g)
		{
			if (Common == null || !Visible || GetScale() == null)
			{
				return;
			}
			g.StartHotRegion(this);
			GetScale().SetDrawRegion(g);
			if (Image != "" && CapImage != "")
			{
				DrawImage(g, primary: true, drawShadow: false);
				DrawImage(g, primary: false, drawShadow: false);
				SetAllHotRegions(g);
				g.RestoreDrawRegion();
				g.EndHotRegion();
				return;
			}
			if (Image != "")
			{
				DrawImage(g, primary: true, drawShadow: false);
			}
			float positionFromValue = GetScale().GetPositionFromValue(base.Position);
			PointF absolutePoint = g.GetAbsolutePoint(GetScale().GetPivotPoint());
			IPen pen = g.ResourceFactory.CreatePen(base.BorderColor, base.BorderWidth);
			pen.DashStyle = g.GetPenStyle(base.BorderStyle);
			if (pen.DashStyle != 0)
			{
				pen.Alignment = PenAlignment.Center;
			}
			KnobStyleAttrib knobStyleAttrib = GetKnobStyleAttrib(g, absolutePoint, positionFromValue);
			try
			{
				if (knobStyleAttrib.paths != null)
				{
					for (int i = 0; i < knobStyleAttrib.paths.Length; i++)
					{
						if (knobStyleAttrib.brushes[i] != null && knobStyleAttrib.paths[i] != null)
						{
							g.FillPath(knobStyleAttrib.brushes[i], knobStyleAttrib.paths[i]);
						}
					}
					if (base.BorderWidth > 0 && knobStyleAttrib.paths[0] != null)
					{
						g.DrawPath(pen, knobStyleAttrib.paths[0]);
					}
					if (knobStyleAttrib.paths[0] != null)
					{
						AddHotRegion(g.ResourceFactory.CreatePath(knobStyleAttrib.paths[0].PathPoints, knobStyleAttrib.paths[0].PathTypes), primary: true);
					}
				}
			}
			finally
			{
				knobStyleAttrib.Dispose();
			}
			if (CapImage != "")
			{
				DrawImage(g, primary: false, drawShadow: false);
			}
			SetAllHotRegions(g);
			g.RestoreDrawRegion();
			g.EndHotRegion();
		}

		internal void DrawImage(GaugeGraphics g, bool primary, bool drawShadow)
		{
			if (!Visible || (drawShadow && base.ShadowOffset == 0f))
			{
				return;
			}
			float width = Width;
			width = g.GetAbsoluteDimension(width);
			Image image = null;
			image = ((!primary) ? Common.ImageLoader.LoadImage(CapImage) : Common.ImageLoader.LoadImage(Image));
			if (image.Width == 0 || image.Height == 0)
			{
				return;
			}
			Point empty = Point.Empty;
			empty = ((!primary) ? CapImageOrigin : ImageOrigin);
			if (empty.IsEmpty)
			{
				empty.X = image.Width / 2;
				empty.Y = image.Height / 2;
			}
			int num = (image.Height <= image.Width) ? image.Width : image.Height;
			if (num != 0)
			{
				float num2 = (!primary) ? (g.GetAbsoluteDimension(CapWidth * 2f) / (float)num) : (g.GetAbsoluteDimension(Width * 2f) / (float)num);
				Rectangle rectangle = new Rectangle(0, 0, (int)((float)image.Width * num2), (int)((float)image.Height * num2));
				IImageDrawOptions imageAttributes = g.ResourceFactory.CreateImageDrawOptions();
				if (primary && ImageTransColor != Color.Empty)
				{
					imageAttributes.SetTransparentColor(ImageTransColor);
				}
				if (!primary && CapImageTransColor != Color.Empty)
				{
					imageAttributes.SetTransparentColor(CapImageTransColor);
				}
				Matrix transform = g.Transform;
				Matrix matrix = g.Transform.Clone();
				float positionFromValue = GetScale().GetPositionFromValue(base.Position);
				PointF absolutePoint = g.GetAbsolutePoint(GetScale().GetPivotPoint());
				PointF pointF = new PointF((float)empty.X * num2, (float)empty.Y * num2);
				float offsetX = matrix.OffsetX;
				float offsetY = matrix.OffsetY;
				matrix.Translate(absolutePoint.X - pointF.X, absolutePoint.Y - pointF.Y, MatrixOrder.Append);
				absolutePoint.X += offsetX;
				absolutePoint.Y += offsetY;
				matrix.RotateAt(positionFromValue, absolutePoint, MatrixOrder.Append);
				if (drawShadow)
				{
					imageAttributes.SetChannelScale(0f, 0f, 0f, Common.GaugeCore.ShadowIntensity / 100f);
					matrix.Translate(base.ShadowOffset, base.ShadowOffset, MatrixOrder.Append);
				}
				else if (primary && !ImageHueColor.IsEmpty)
				{
					Color color = g.TransformHueColor(ImageHueColor);
					imageAttributes.SetChannelScale((float)(int)color.R / 255f, (float)(int)color.G / 255f, (float)(int)color.B / 255f, 1f);
				}
				else if (!primary && !CapImageHueColor.IsEmpty)
				{
					Color color2 = g.TransformHueColor(CapImageHueColor);
					imageAttributes.SetChannelScale((float)(int)color2.R / 255f, (float)(int)color2.G / 255f, (float)(int)color2.B / 255f, 1f);
				}
				g.Transform = matrix;
				ImageSmoothingState imageSmoothingState = new ImageSmoothingState(g);
				imageSmoothingState.Set();
				g.DrawImage(g.ResourceFactory.WrapImage(image), rectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				imageSmoothingState.Restore();
				g.Transform = transform;
				if (!drawShadow)
				{
					matrix.Translate(0f - offsetX, 0f - offsetY, MatrixOrder.Append);
					GraphicsPath graphicsPath = new GraphicsPath();
					graphicsPath.AddRectangle(rectangle);
					graphicsPath.Transform(matrix);
					AddHotRegion(g.ResourceFactory.WrapPath(graphicsPath), primary);
				}
			}
		}

		internal GraphicsPath GetPointerPath(GaugeGraphics g)
		{
			if (!Visible)
			{
				return null;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			float positionFromValue = GetScale().GetPositionFromValue(base.Position);
			PointF absolutePoint = g.GetAbsolutePoint(GetScale().GetPivotPoint());
			KnobStyleAttrib knobStyleAttrib = GetKnobStyleAttrib(g, absolutePoint, positionFromValue);
			if (knobStyleAttrib.paths != null && knobStyleAttrib.paths[0] != null)
			{
				graphicsPath.AddPath(g.ResourceFactory.UnwrapPath(knobStyleAttrib.paths[0]), connect: false);
			}
			return graphicsPath;
		}

		internal GraphicsPath GetShadowPath(GaugeGraphics g)
		{
			if (base.ShadowOffset == 0f || GetScale() == null)
			{
				return null;
			}
			GetScale().SetDrawRegion(g);
			if (Image != "")
			{
				DrawImage(g, primary: true, drawShadow: true);
			}
			if (CapImage != "")
			{
				DrawImage(g, primary: false, drawShadow: true);
			}
			GraphicsPath pointerPath = GetPointerPath(g);
			if (pointerPath == null || pointerPath.PointCount == 0)
			{
				g.RestoreDrawRegion();
				return null;
			}
			using (Matrix matrix = new Matrix())
			{
				matrix.Translate(base.ShadowOffset, base.ShadowOffset);
				pointerPath.Transform(matrix);
			}
			PointF pointF = new PointF(g.Graphics.Transform.OffsetX, g.Graphics.Transform.OffsetY);
			g.RestoreDrawRegion();
			PointF pointF2 = new PointF(g.Graphics.Transform.OffsetX, g.Graphics.Transform.OffsetY);
			using (Matrix matrix2 = new Matrix())
			{
				matrix2.Translate(pointF.X - pointF2.X, pointF.Y - pointF2.Y);
				pointerPath.Transform(matrix2);
				return pointerPath;
			}
		}

		internal void AddHotRegion(IGraphicsPath path, bool primary)
		{
			if (primary)
			{
				hotRegions[0] = path;
			}
			else
			{
				hotRegions[1] = path;
			}
		}

		internal void SetAllHotRegions(GaugeGraphics g)
		{
			Common.GaugeCore.HotRegionList.SetHotRegion(this, g.GetAbsolutePoint(GetScale().GetPivotPoint()), hotRegions[0], hotRegions[1]);
			hotRegions[0]?.Dispose();
			hotRegions[1]?.Dispose();
			hotRegions[0] = null;
			hotRegions[1] = null;
		}

		public override string ToString()
		{
			return Name;
		}

		public CircularGauge GetGauge()
		{
			if (Collection == null)
			{
				return null;
			}
			return (CircularGauge)Collection.parent;
		}

		public CircularScale GetScale()
		{
			if (GetGauge() == null)
			{
				return null;
			}
			if (ScaleName == string.Empty)
			{
				return null;
			}
			if (ScaleName == "Default" && GetGauge().Scales.Count == 0)
			{
				return null;
			}
			return GetGauge().Scales[ScaleName];
		}

		void ISelectable.DrawSelection(GaugeGraphics g, bool designTimeSelection)
		{
			if (GetGauge() == null || GetScale() == null)
			{
				return;
			}
			Stack stack = new Stack();
			for (NamedElement namedElement = GetGauge().ParentObject; namedElement != null; namedElement = (NamedElement)((IRenderable)namedElement).GetParentRenderable())
			{
				stack.Push(namedElement);
			}
			foreach (IRenderable item in stack)
			{
				g.CreateDrawRegion(item.GetBoundRect(g));
			}
			g.CreateDrawRegion(((IRenderable)GetGauge()).GetBoundRect(g));
			GetScale().SetDrawRegion(g);
			using (GraphicsPath graphicsPath = GetPointerPath(g))
			{
				if (graphicsPath != null)
				{
					RectangleF bounds = graphicsPath.GetBounds();
					g.DrawSelection(bounds, designTimeSelection, Common.GaugeCore.SelectionBorderColor, Common.GaugeCore.SelectionMarkerColor);
				}
			}
			g.RestoreDrawRegion();
			g.RestoreDrawRegion();
			foreach (IRenderable item2 in stack)
			{
				_ = item2;
				g.RestoreDrawRegion();
			}
		}

		public override object Clone()
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatSerializer binaryFormatSerializer = new BinaryFormatSerializer();
			binaryFormatSerializer.Serialize(this, stream);
			Knob knob = new Knob();
			binaryFormatSerializer.Deserialize(knob, stream);
			return knob;
		}
	}
}
