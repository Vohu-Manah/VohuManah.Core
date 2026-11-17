using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Numerics;
using Reporting.Rendering;
using Reporting.Gauge.WebForms.Rendering;

namespace Reporting.Gauge.WebForms
{
	internal class GaugeGraphics : RenderingEngine
	{
		internal CommonElements common;

		private Pen pen;

		private SolidBrush solidBrush;

		private float width;

		private float height;

		internal bool softShadows = true;

		private AntiAliasing antiAliasing = AntiAliasing.All;

		internal bool IsMetafile;

		internal PointF InitialOffset = new PointF(0f, 0f);

		private Stack graphicStates = new Stack();

		public new Graphics Graphics
		{
			get
			{
				return base.Graphics;
			}
			set
			{
				if (base.Graphics != value)
				{
					base.Graphics = value;
					if (base.Graphics.Transform != null)
					{
						InitialOffset.X = base.Graphics.Transform.OffsetX;
						InitialOffset.Y = base.Graphics.Transform.OffsetY;
					}
				}
			}
		}

		internal AntiAliasing AntiAliasing
		{
			get
			{
				return antiAliasing;
			}
			set
			{
				antiAliasing = value;
				if (Graphics != null)
				{
					if ((antiAliasing & AntiAliasing.Graphics) == AntiAliasing.Graphics)
					{
						base.SmoothingMode = SmoothingMode.AntiAlias;
					}
					else
					{
						base.SmoothingMode = SmoothingMode.None;
					}
				}
			}
		}

		internal static Brush GetHatchBrush(GaugeHatchStyle hatchStyle, Color backColor, Color foreColor)
		{
			return new HatchBrush((HatchStyle)Enum.Parse(typeof(HatchStyle), hatchStyle.ToString()), foreColor, backColor);
		}

		/// <summary>
		/// Interface-typed sibling of <see cref="GetHatchBrush"/> (Milestone B2 equivalent, dual-overload
		/// strategy per tasks/gauge-gdi-type-abstraction.md). Instance method (not static like the original)
		/// since it needs <see cref="RenderingEngine.ResourceFactory"/>. Self-contained — no shared field or
		/// image-loading dependency — so this converts cleanly, unlike <see cref="GetTextureBrush"/>.
		/// </summary>
		internal IHatchBrush GetHatchBrushResource(GaugeHatchStyle hatchStyle, Color backColor, Color foreColor)
		{
			return ResourceFactory.CreateHatchBrush((HatchStyle)Enum.Parse(typeof(HatchStyle), hatchStyle.ToString()), foreColor, backColor);
		}

		internal Brush GetTextureBrush(string name, Color backImageTranspColor, GaugeImageWrapMode mode)
		{
			Image image = common.ImageLoader.LoadImage(name);
			ImageAttributes imageAttributes = new ImageAttributes();
			imageAttributes.SetWrapMode((WrapMode)((mode == GaugeImageWrapMode.Unscaled) ? GaugeImageWrapMode.Scaled : mode));
			if (backImageTranspColor != Color.Empty)
			{
				imageAttributes.SetColorKey(backImageTranspColor, backImageTranspColor, ColorAdjustType.Default);
			}
			return new TextureBrush(image, new RectangleF(0f, 0f, image.Width, image.Height), imageAttributes);
		}

		/// <summary>
		/// Interface-typed sibling of <see cref="GetTextureBrush"/> (Milestone B2 equivalent, dual-overload
		/// strategy per tasks/gauge-gdi-type-abstraction.md). <c>common.ImageLoader</c> itself remains
		/// GDI+-only/concrete (documented, deliberate) — its loaded <see cref="Image"/> is bridged into
		/// <see cref="IChartImage"/> via <see cref="RenderingEngine.ResourceFactory"/>'s <c>WrapImage</c>.
		/// </summary>
		internal ITextureBrush GetTextureBrushResource(string name, Color backImageTranspColor, GaugeImageWrapMode mode)
		{
			Image image = common.ImageLoader.LoadImage(name);
			IImageDrawOptions imageAttributes = ResourceFactory.CreateImageDrawOptions();
			imageAttributes.SetWrapMode((WrapMode)((mode == GaugeImageWrapMode.Unscaled) ? GaugeImageWrapMode.Scaled : mode));
			if (backImageTranspColor != Color.Empty)
			{
				imageAttributes.SetTransparentColor(backImageTranspColor);
			}
			return ResourceFactory.CreateTextureBrush(ResourceFactory.WrapImage(image), new RectangleF(0f, 0f, image.Width, image.Height), imageAttributes);
		}

		internal Brush GetShadowBrush()
		{
			return new SolidBrush(GetShadowColor());
		}

		/// <summary>Interface-typed sibling of <see cref="GetShadowBrush"/> (dual-overload, additive).</summary>
		internal ISolidBrush GetShadowBrushResource()
		{
			return ResourceFactory.CreateSolidBrush(GetShadowColor());
		}

		internal Color GetShadowColor()
		{
			return Color.FromArgb((int)(255f * common.GaugeCore.ShadowIntensity / 100f), Color.Black);
		}

		internal Brush GetGradientBrush(RectangleF rectangle, Color firstColor, Color secondColor, GradientType type)
		{
			rectangle.Inflate(1f, 1f);
			Brush brush = null;
			float angle = 0f;
			if (rectangle.Height == 0f || rectangle.Width == 0f)
			{
				return new SolidBrush(Color.Black);
			}
			switch (type)
			{
			case GradientType.LeftRight:
			case GradientType.VerticalCenter:
				angle = 0f;
				break;
			case GradientType.TopBottom:
			case GradientType.HorizontalCenter:
				angle = 90f;
				break;
			case GradientType.DiagonalLeft:
				angle = (float)(Math.Atan(rectangle.Width / rectangle.Height) * 180.0 / Math.PI);
				break;
			case GradientType.DiagonalRight:
				angle = (float)(180.0 - Math.Atan(rectangle.Width / rectangle.Height) * 180.0 / Math.PI);
				break;
			}
			if (type == GradientType.TopBottom || type == GradientType.LeftRight || type == GradientType.DiagonalLeft || type == GradientType.DiagonalRight || type == GradientType.HorizontalCenter || type == GradientType.VerticalCenter)
			{
				RectangleF rect = new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
				switch (type)
				{
				case GradientType.HorizontalCenter:
					rect.Height /= 2f;
					brush = new LinearGradientBrush(rect, firstColor, secondColor, angle);
					((LinearGradientBrush)brush).WrapMode = WrapMode.TileFlipX;
					break;
				case GradientType.VerticalCenter:
					rect.Width /= 2f;
					brush = new LinearGradientBrush(rect, firstColor, secondColor, angle);
					((LinearGradientBrush)brush).WrapMode = WrapMode.TileFlipX;
					break;
				default:
					brush = new LinearGradientBrush(rectangle, firstColor, secondColor, angle);
					break;
				}
				return brush;
			}
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddRectangle(rectangle);
			brush = new PathGradientBrush(graphicsPath);
			((PathGradientBrush)brush).CenterColor = firstColor;
			((PathGradientBrush)brush).CenterPoint = new PointF(rectangle.X + rectangle.Width / 2f, rectangle.Y + rectangle.Height / 2f);
			Color[] surroundColors = new Color[1]
			{
				secondColor
			};
			((PathGradientBrush)brush).SurroundColors = surroundColors;
			graphicsPath?.Dispose();
			return brush;
		}

		/// <summary>
		/// Interface-typed sibling of <see cref="GetGradientBrush"/> (Milestone B2 equivalent, dual-overload
		/// strategy). Self-contained — its only geometry (the path-gradient branch's rectangle) is built
		/// locally via <see cref="RenderingEngine.ResourceFactory"/>, no shared field or image dependency.
		/// Mixed return shape (linear vs. path-gradient) mirrors the original's mixed concrete <c>Brush</c> return.
		/// </summary>
		internal IBrush GetGradientBrushResource(RectangleF rectangle, Color firstColor, Color secondColor, GradientType type)
		{
			rectangle.Inflate(1f, 1f);
			float angle = 0f;
			if (rectangle.Height == 0f || rectangle.Width == 0f)
			{
				return ResourceFactory.CreateSolidBrush(Color.Black);
			}
			switch (type)
			{
			case GradientType.LeftRight:
			case GradientType.VerticalCenter:
				angle = 0f;
				break;
			case GradientType.TopBottom:
			case GradientType.HorizontalCenter:
				angle = 90f;
				break;
			case GradientType.DiagonalLeft:
				angle = (float)(Math.Atan(rectangle.Width / rectangle.Height) * 180.0 / Math.PI);
				break;
			case GradientType.DiagonalRight:
				angle = (float)(180.0 - Math.Atan(rectangle.Width / rectangle.Height) * 180.0 / Math.PI);
				break;
			}
			if (type == GradientType.TopBottom || type == GradientType.LeftRight || type == GradientType.DiagonalLeft || type == GradientType.DiagonalRight || type == GradientType.HorizontalCenter || type == GradientType.VerticalCenter)
			{
				RectangleF rect = new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
				ILinearGradientBrush linearBrush;
				switch (type)
				{
				case GradientType.HorizontalCenter:
					rect.Height /= 2f;
					linearBrush = ResourceFactory.CreateLinearGradientBrush(rect, firstColor, secondColor, angle);
					linearBrush.WrapMode = WrapMode.TileFlipX;
					break;
				case GradientType.VerticalCenter:
					rect.Width /= 2f;
					linearBrush = ResourceFactory.CreateLinearGradientBrush(rect, firstColor, secondColor, angle);
					linearBrush.WrapMode = WrapMode.TileFlipX;
					break;
				default:
					linearBrush = ResourceFactory.CreateLinearGradientBrush(rectangle, firstColor, secondColor, angle);
					break;
				}
				return linearBrush;
			}
			IGraphicsPath resourcePath = ResourceFactory.CreatePath();
			resourcePath.AddRectangle(rectangle);
			IPathGradientBrush pathBrush = ResourceFactory.CreatePathGradientBrush(resourcePath);
			pathBrush.CenterColor = firstColor;
			pathBrush.CenterPoint = new PointF(rectangle.X + rectangle.Width / 2f, rectangle.Y + rectangle.Height / 2f);
			pathBrush.SurroundColors = new Color[1]
			{
				secondColor
			};
			resourcePath.Dispose();
			return pathBrush;
		}

		internal Brush GetPieGradientBrush(RectangleF rectangle, Color firstColor, Color secondColor)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddEllipse(rectangle);
			PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath);
			pathGradientBrush.CenterColor = firstColor;
			pathGradientBrush.CenterPoint = new PointF(rectangle.X + rectangle.Width / 2f, rectangle.Y + rectangle.Height / 2f);
			Color[] array2 = pathGradientBrush.SurroundColors = new Color[1]
			{
				secondColor
			};
			graphicsPath?.Dispose();
			return pathGradientBrush;
		}

		/// <summary>
		/// Interface-typed sibling of <see cref="GetPieGradientBrush"/> (Milestone B2 equivalent, dual-overload
		/// strategy). Self-contained — its ellipse geometry is built locally via
		/// <see cref="RenderingEngine.ResourceFactory"/>, no shared field or image dependency.
		/// </summary>
		internal IPathGradientBrush GetPieGradientBrushResource(RectangleF rectangle, Color firstColor, Color secondColor)
		{
			IGraphicsPath graphicsPath = ResourceFactory.CreatePath();
			graphicsPath.AddEllipse(rectangle);
			IPathGradientBrush pathGradientBrush = ResourceFactory.CreatePathGradientBrush(graphicsPath);
			pathGradientBrush.CenterColor = firstColor;
			pathGradientBrush.CenterPoint = new PointF(rectangle.X + rectangle.Width / 2f, rectangle.Y + rectangle.Height / 2f);
			pathGradientBrush.SurroundColors = new Color[1]
			{
				secondColor
			};
			graphicsPath.Dispose();
			return pathGradientBrush;
		}

		internal DashStyle GetPenStyle(GaugeDashStyle style)
		{
			switch (style)
			{
			case GaugeDashStyle.Dash:
				return DashStyle.Dash;
			case GaugeDashStyle.DashDot:
				return DashStyle.DashDot;
			case GaugeDashStyle.DashDotDot:
				return DashStyle.DashDotDot;
			case GaugeDashStyle.Dot:
				return DashStyle.Dot;
			default:
				return DashStyle.Solid;
			}
		}

		/// <summary>
		/// Interface-typed replacement for the formerly-concrete <c>GetMarkerBrush</c> (removed — dead code
		/// eliminated once <see cref="Knob"/>/<see cref="CircularPointer"/>/<see cref="LinearPointer"/>'s
		/// marker-attrib producers all switched to this sibling during the atomic retyping pass). Unblocked by
		/// <see cref="IGraphicsPath.GetBounds(Matrix3x2)"/>, added specifically to close this gap (GDI+'s
		/// transformed-bounds overload had no equivalent before). The transformed-bounds matrix is built via
		/// the exact same native <see cref="Matrix"/>/<c>RotateAt</c> call as the original, then its element
		/// values are carried into a <see cref="Matrix3x2"/> for the interface call — a literal 1:1 port with
		/// no re-derivation of rotation direction/order, mirroring how <see cref="ILinearGradientBrush.SetRotationTransform"/> was ported.
		/// </summary>
		internal IBrush GetMarkerBrushResource(IGraphicsPath path, MarkerStyle markerStyle, PointF pointOrigin, float angle, Color fillColor, GradientType fillGradientType, Color fillGradientEndColor, GaugeHatchStyle fillHatchStyle)
		{
			IBrush brush;
			if (fillHatchStyle != 0)
			{
				return GetHatchBrushResource(fillHatchStyle, fillColor, fillGradientEndColor);
			}
			if (fillGradientType != 0)
			{
				RectangleF bounds = path.GetBounds();
				if (markerStyle == MarkerStyle.Circle && fillGradientType == GradientType.DiagonalLeft)
				{
					PointF center = new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
					using Matrix nativeMatrix = new Matrix();
					nativeMatrix.RotateAt(45f, center);
					if (bounds.Width != bounds.Height)
					{
						float[] elements = nativeMatrix.Elements;
						bounds = path.GetBounds(new Matrix3x2(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]));
					}
					ILinearGradientBrush linearGradientBrush = (ILinearGradientBrush)GetGradientBrushResource(bounds, fillColor, fillGradientEndColor, GradientType.LeftRight);
					linearGradientBrush.SetRotationTransform(45f, center);
					brush = linearGradientBrush;
				}
				else if (markerStyle == MarkerStyle.Circle && fillGradientType == GradientType.DiagonalRight)
				{
					PointF center2 = new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
					using Matrix nativeMatrix2 = new Matrix();
					nativeMatrix2.RotateAt(135f, center2);
					if (bounds.Width != bounds.Height)
					{
						float[] elements2 = nativeMatrix2.Elements;
						bounds = path.GetBounds(new Matrix3x2(elements2[0], elements2[1], elements2[2], elements2[3], elements2[4], elements2[5]));
					}
					ILinearGradientBrush linearGradientBrush2 = (ILinearGradientBrush)GetGradientBrushResource(bounds, fillColor, fillGradientEndColor, GradientType.TopBottom);
					linearGradientBrush2.SetRotationTransform(135f, center2);
					brush = linearGradientBrush2;
				}
				else if (markerStyle == MarkerStyle.Circle && fillGradientType == GradientType.Center)
				{
					bounds.Inflate(1f, 1f);
					using IGraphicsPath graphicsPath = ResourceFactory.CreatePath();
					graphicsPath.AddArc(bounds.X, bounds.Y, bounds.Width, bounds.Height, 0f, 360f);
					IPathGradientBrush pathGradientBrush = ResourceFactory.CreatePathGradientBrush(graphicsPath);
					pathGradientBrush.CenterColor = fillColor;
					pathGradientBrush.CenterPoint = new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
					pathGradientBrush.SurroundColors = new Color[1] { fillGradientEndColor };
					brush = pathGradientBrush;
				}
				else
				{
					brush = GetGradientBrushResource(path.GetBounds(), fillColor, fillGradientEndColor, fillGradientType);
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
				brush = ResourceFactory.CreateSolidBrush(fillColor);
			}
			return brush;
		}

		internal GraphicsPath CreateMarker(PointF point, float markerWidth, float markerHeight, MarkerStyle markerStyle)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			RectangleF empty = RectangleF.Empty;
			empty.X = point.X - markerWidth / 2f;
			empty.Y = point.Y - markerHeight / 2f;
			empty.Width = markerWidth;
			empty.Height = markerHeight;
			switch (markerStyle)
			{
			case MarkerStyle.Circle:
				graphicsPath.AddEllipse(empty);
				break;
			case MarkerStyle.Diamond:
			{
				PointF[] array = new PointF[4];
				array[0].X = empty.X;
				array[0].Y = empty.Y + empty.Height / 2f;
				array[1].X = empty.X + empty.Width / 2f;
				array[1].Y = empty.Top;
				array[2].X = empty.Right;
				array[2].Y = empty.Y + empty.Height / 2f;
				array[3].X = empty.X + empty.Width / 2f;
				array[3].Y = empty.Bottom;
				graphicsPath.AddPolygon(array);
				break;
			}
			case MarkerStyle.Star:
				graphicsPath.AddPolygon(CreateStarPolygon(empty, 5));
				break;
			case MarkerStyle.None:
			case MarkerStyle.Rectangle:
			{
				PointF[] array = new PointF[4];
				array[0].X = empty.X;
				array[0].Y = empty.Y;
				array[1].X = empty.X + empty.Width;
				array[1].Y = empty.Y;
				array[2].X = empty.X + empty.Width;
				array[2].Y = empty.Y + empty.Height;
				array[3].X = empty.X;
				array[3].Y = empty.Y + empty.Height;
				graphicsPath.AddPolygon(array);
				break;
			}
			case MarkerStyle.Trapezoid:
			{
				PointF[] array = new PointF[4];
				array[0].X = empty.X;
				array[0].Y = empty.Bottom;
				array[1].X = empty.X + empty.Width / 4f;
				array[1].Y = empty.Top;
				array[2].X = empty.X + empty.Width / 4f * 3f;
				array[2].Y = empty.Top;
				array[3].X = empty.Right;
				array[3].Y = empty.Bottom;
				graphicsPath.AddPolygon(array);
				break;
			}
			case MarkerStyle.Triangle:
			{
				PointF[] array = new PointF[3];
				array[0].X = empty.X;
				array[0].Y = empty.Bottom;
				array[1].X = empty.X + empty.Width / 2f;
				array[1].Y = empty.Top;
				array[2].X = empty.Right;
				array[2].Y = empty.Bottom;
				graphicsPath.AddPolygon(array);
				break;
			}
			case MarkerStyle.Wedge:
			{
				if (empty.Width >= empty.Height)
				{
					graphicsPath = CreateMarker(point, markerWidth, markerHeight, MarkerStyle.Triangle);
					break;
				}
				float num4 = (float)Math.Pow(Math.Pow(empty.Width, 2.0) - Math.Pow(empty.Width / 2f, 2.0), 0.5);
				PointF[] array = new PointF[5];
				array[0].X = empty.X;
				array[0].Y = empty.Y + num4;
				array[1].X = empty.X + empty.Width / 2f;
				array[1].Y = empty.Y;
				array[2].X = empty.X + empty.Width;
				array[2].Y = empty.Y + num4;
				array[3].X = empty.X + empty.Width;
				array[3].Y = empty.Y + empty.Height;
				array[4].X = empty.X;
				array[4].Y = empty.Y + empty.Height;
				graphicsPath.AddPolygon(array);
				break;
			}
			case MarkerStyle.Pentagon:
			{
				float y = (float)Math.Cos(Math.PI * 2.0 / 5.0);
				float num = (float)Math.Cos(Math.PI / 5.0);
				float num2 = (float)Math.Sin(Math.PI * 2.0 / 5.0);
				float num3 = (float)Math.Sin(Math.PI * 4.0 / 5.0);
				PointF[] array = new PointF[5];
				array[0].X = 0f;
				array[0].Y = 1f;
				array[1].X = num2;
				array[1].Y = y;
				array[2].X = num3;
				array[2].Y = 0f - num;
				array[3].X = 0f - num3;
				array[3].Y = 0f - num;
				array[4].X = 0f - num2;
				array[4].Y = y;
				using (Matrix matrix = new Matrix())
				{
					matrix.Scale(markerWidth / 2f, markerHeight / 2f);
					matrix.TransformPoints(array);
					matrix.Reset();
					matrix.Rotate(180f);
					matrix.TransformPoints(array);
					matrix.Reset();
					matrix.Translate(point.X, point.Y);
					matrix.TransformPoints(array);
				}
				graphicsPath.AddPolygon(array);
				break;
			}
			default:
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionInvalidMarkerType"));
			}
			return graphicsPath;
		}

		internal PointF[] CreateStarPolygon(RectangleF rectReal, int numberOfCorners)
		{
			bool flag = true;
			PointF[] array = new PointF[numberOfCorners * 2];
			PointF[] array2 = new PointF[1];
			RectangleF rectangleF = new RectangleF(0f, 0f, 1f, 1f);
			using (Matrix matrix = new Matrix())
			{
				for (int i = 0; i < numberOfCorners * 2; i++)
				{
					array2[0] = new PointF(rectangleF.X + rectangleF.Width / 2f, flag ? rectangleF.Y : (rectangleF.Y + rectangleF.Height / 4f));
					matrix.Reset();
					matrix.RotateAt((float)i * (360f / ((float)numberOfCorners * 2f)), new PointF(rectangleF.X + rectangleF.Width / 2f, rectangleF.Y + rectangleF.Height / 2f));
					matrix.TransformPoints(array2);
					array[i] = array2[0];
					flag = !flag;
				}
				matrix.Reset();
				matrix.Scale(rectReal.Width, rectReal.Height);
				matrix.TransformPoints(array);
				matrix.Reset();
				matrix.Translate(rectReal.X, rectReal.Y);
				matrix.TransformPoints(array);
				return array;
			}
		}

		internal void DrawPathShadowAbs(GraphicsPath path, Color shadowColor, float shadowWidth)
		{
			if (shadowWidth != 0f)
			{
				Matrix matrix = new Matrix();
				matrix.Translate(shadowWidth, shadowWidth);
				path.Transform(matrix);
				using (Brush brush = new SolidBrush(shadowColor))
				{
					FillPath(brush, path);
				}
				matrix.Reset();
				matrix.Translate(0f - shadowWidth, 0f - shadowWidth);
				path.Transform(matrix);
			}
		}

		internal void DrawPathShadowAbs(IGraphicsPath path, Color shadowColor, float shadowWidth)
		{
			if (shadowWidth != 0f)
			{
				path.Transform(Matrix3x2.CreateTranslation(shadowWidth, shadowWidth));
				using (IBrush brush = ResourceFactory.CreateSolidBrush(shadowColor))
				{
					FillPath(brush, path);
				}
				path.Transform(Matrix3x2.CreateTranslation(0f - shadowWidth, 0f - shadowWidth));
			}
		}

		internal void DrawPathAbs(GraphicsPath path, Color backColor, GaugeHatchStyle backHatchStyle, string backImage, GaugeImageWrapMode backImageMode, Color backImageTranspColor, GaugeImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, GaugeDashStyle borderStyle, PenAlignment penAlignment)
		{
			Brush brush = null;
			Brush brush2 = null;
			if (backColor.IsEmpty)
			{
				backColor = Color.White;
			}
			if (backGradientEndColor.IsEmpty)
			{
				backGradientEndColor = Color.White;
			}
			if (borderColor.IsEmpty)
			{
				borderColor = Color.White;
				borderWidth = 0;
			}
			pen.Color = borderColor;
			pen.Width = borderWidth;
			pen.Alignment = penAlignment;
			pen.DashStyle = GetPenStyle(borderStyle);
			if (backGradientType == GradientType.None)
			{
				solidBrush.Color = backColor;
				brush = solidBrush;
			}
			else
			{
				RectangleF bounds = path.GetBounds();
				bounds.Inflate(new SizeF(2f, 2f));
				brush = GetGradientBrush(bounds, backColor, backGradientEndColor, backGradientType);
			}
			if (backHatchStyle != 0)
			{
				brush = GetHatchBrush(backHatchStyle, backColor, backGradientEndColor);
			}
			if (backImage.Length > 0 && backImageMode != GaugeImageWrapMode.Unscaled && backImageMode != GaugeImageWrapMode.Scaled)
			{
				brush2 = brush;
				brush = GetTextureBrush(backImage, backImageTranspColor, backImageMode);
			}
			RectangleF bounds2 = path.GetBounds();
			if (backImage.Length > 0 && (backImageMode == GaugeImageWrapMode.Unscaled || backImageMode == GaugeImageWrapMode.Scaled))
			{
				Image image = common.ImageLoader.LoadImage(backImage);
				ImageAttributes imageAttributes = new ImageAttributes();
				if (backImageTranspColor != Color.Empty)
				{
					imageAttributes.SetColorKey(backImageTranspColor, backImageTranspColor, ColorAdjustType.Default);
				}
				RectangleF rectangleF = default(RectangleF);
				rectangleF.X = bounds2.X;
				rectangleF.Y = bounds2.Y;
				rectangleF.Width = bounds2.Width;
				rectangleF.Height = bounds2.Height;
				if (backImageMode == GaugeImageWrapMode.Unscaled)
				{
					rectangleF.Width = image.Width;
					rectangleF.Height = image.Height;
					if (rectangleF.Width < bounds2.Width)
					{
						switch (backImageAlign)
						{
						case GaugeImageAlign.TopRight:
						case GaugeImageAlign.Right:
						case GaugeImageAlign.BottomRight:
							rectangleF.X = bounds2.Right - rectangleF.Width;
							break;
						case GaugeImageAlign.Top:
						case GaugeImageAlign.Bottom:
						case GaugeImageAlign.Center:
							rectangleF.X = bounds2.X + (bounds2.Width - rectangleF.Width) / 2f;
							break;
						}
					}
					if (rectangleF.Height < bounds2.Height)
					{
						switch (backImageAlign)
						{
						case GaugeImageAlign.BottomRight:
						case GaugeImageAlign.Bottom:
						case GaugeImageAlign.BottomLeft:
							rectangleF.Y = bounds2.Bottom - rectangleF.Height;
							break;
						case GaugeImageAlign.Right:
						case GaugeImageAlign.Left:
						case GaugeImageAlign.Center:
							rectangleF.Y = bounds2.Y + (bounds2.Height - rectangleF.Height) / 2f;
							break;
						}
					}
				}
				FillPath(brush, path);
				Region clip = base.Clip;
				base.Clip = new Region(path);
				DrawImage(image, new Rectangle((int)Math.Round(rectangleF.X), (int)Math.Round(rectangleF.Y), (int)Math.Round(rectangleF.Width), (int)Math.Round(rectangleF.Height)), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				base.Clip = clip;
			}
			else
			{
				if (brush2 != null && backImageTranspColor != Color.Empty)
				{
					FillPath(brush2, path);
				}
				FillPath(brush, path);
			}
			if (borderColor != Color.Empty && borderWidth > 0 && borderStyle != 0)
			{
				DrawPath(pen, path);
			}
		}

		/// <summary>
		/// Interface-typed counterpart of <see cref="DrawPathAbs(GraphicsPath, Color, GaugeHatchStyle, string, GaugeImageWrapMode, Color, GaugeImageAlign, GradientType, Color, Color, int, GaugeDashStyle, PenAlignment)"/>
		/// (Milestone A4 — coexists until a real caller builds its path via <see cref="IGraphicsPath"/>;
		/// see tasks/gauge-gdi-type-abstraction.md). Unlike the concrete overload, this one does not
		/// touch the shared <c>pen</c>/<c>solidBrush</c> fields — it constructs its own local
		/// <see cref="IPen"/>/<see cref="IBrush"/> resources via <see cref="RenderingEngine.ResourceFactory"/>,
		/// which is what avoids the ripple that would otherwise force every real caller (all of which still
		/// build their paths as concrete <see cref="GraphicsPath"/>) to convert their own path-building code
		/// at the same time. Mirrors Chart's identically-shaped `DrawPathAbs(IGraphicsPath, ...)` exactly,
		/// including reusing its clip-swap/`DrawImage` pattern now that both are available here too.
		/// </summary>
		internal void DrawPathAbs(IGraphicsPath path, Color backColor, GaugeHatchStyle backHatchStyle, string backImage, GaugeImageWrapMode backImageMode, Color backImageTranspColor, GaugeImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor, Color borderColor, int borderWidth, GaugeDashStyle borderStyle, PenAlignment penAlignment)
		{
			IBrush brush = null;
			IBrush brush2 = null;
			if (backColor.IsEmpty)
			{
				backColor = Color.White;
			}
			if (backGradientEndColor.IsEmpty)
			{
				backGradientEndColor = Color.White;
			}
			if (borderColor.IsEmpty)
			{
				borderColor = Color.White;
				borderWidth = 0;
			}
			IPen pathPen = ResourceFactory.CreatePen(borderColor, borderWidth);
			pathPen.Alignment = penAlignment;
			pathPen.DashStyle = GetPenStyle(borderStyle);
			if (backGradientType == GradientType.None)
			{
				brush = ResourceFactory.CreateSolidBrush(backColor);
			}
			else
			{
				RectangleF bounds = path.GetBounds();
				bounds.Inflate(new SizeF(2f, 2f));
				brush = GetGradientBrushResource(bounds, backColor, backGradientEndColor, backGradientType);
			}
			if (backHatchStyle != 0)
			{
				brush = GetHatchBrushResource(backHatchStyle, backColor, backGradientEndColor);
			}
			if (backImage.Length > 0 && backImageMode != GaugeImageWrapMode.Unscaled && backImageMode != GaugeImageWrapMode.Scaled)
			{
				brush2 = brush;
				brush = GetTextureBrushResource(backImage, backImageTranspColor, backImageMode);
			}
			RectangleF bounds2 = path.GetBounds();
			if (backImage.Length > 0 && (backImageMode == GaugeImageWrapMode.Unscaled || backImageMode == GaugeImageWrapMode.Scaled))
			{
				Image image = common.ImageLoader.LoadImage(backImage);
				IImageDrawOptions imageAttributes = ResourceFactory.CreateImageDrawOptions();
				if (backImageTranspColor != Color.Empty)
				{
					imageAttributes.SetTransparentColor(backImageTranspColor);
				}
				RectangleF rectangleF = default(RectangleF);
				rectangleF.X = bounds2.X;
				rectangleF.Y = bounds2.Y;
				rectangleF.Width = bounds2.Width;
				rectangleF.Height = bounds2.Height;
				if (backImageMode == GaugeImageWrapMode.Unscaled)
				{
					rectangleF.Width = image.Width;
					rectangleF.Height = image.Height;
					if (rectangleF.Width < bounds2.Width)
					{
						switch (backImageAlign)
						{
						case GaugeImageAlign.TopRight:
						case GaugeImageAlign.Right:
						case GaugeImageAlign.BottomRight:
							rectangleF.X = bounds2.Right - rectangleF.Width;
							break;
						case GaugeImageAlign.Top:
						case GaugeImageAlign.Bottom:
						case GaugeImageAlign.Center:
							rectangleF.X = bounds2.X + (bounds2.Width - rectangleF.Width) / 2f;
							break;
						}
					}
					if (rectangleF.Height < bounds2.Height)
					{
						switch (backImageAlign)
						{
						case GaugeImageAlign.BottomRight:
						case GaugeImageAlign.Bottom:
						case GaugeImageAlign.BottomLeft:
							rectangleF.Y = bounds2.Bottom - rectangleF.Height;
							break;
						case GaugeImageAlign.Right:
						case GaugeImageAlign.Left:
						case GaugeImageAlign.Center:
							rectangleF.Y = bounds2.Y + (bounds2.Height - rectangleF.Height) / 2f;
							break;
						}
					}
				}
				FillPath(brush, path);
				IGaugeClipRegion originalClip = GetClipRegion();
				IGaugeClipRegion pathClip = ResourceFactory.CreateRegion(path);
				SetClipRegion(pathClip);
				DrawImage(ResourceFactory.WrapImage(image), new Rectangle((int)Math.Round(rectangleF.X), (int)Math.Round(rectangleF.Y), (int)Math.Round(rectangleF.Width), (int)Math.Round(rectangleF.Height)), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				SetClipRegion(originalClip);
				pathClip.Dispose();
			}
			else
			{
				if (brush2 != null && backImageTranspColor != Color.Empty)
				{
					FillPath(brush2, path);
				}
				FillPath(brush, path);
			}
			if (borderColor != Color.Empty && borderWidth > 0 && borderStyle != 0)
			{
				DrawPath(pathPen, path);
			}
		}

		internal Brush CreateBrush(RectangleF rect, Color backColor, GaugeHatchStyle backHatchStyle, string backImage, GaugeImageWrapMode backImageMode, Color backImageTranspColor, GaugeImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor)
		{
			Brush result = new SolidBrush(backColor);
			if (backImage.Length > 0 && backImageMode != GaugeImageWrapMode.Unscaled && backImageMode != GaugeImageWrapMode.Scaled)
			{
				result = GetTextureBrush(backImage, backImageTranspColor, backImageMode);
			}
			else if (backHatchStyle != 0)
			{
				result = GetHatchBrush(backHatchStyle, backColor, backGradientEndColor);
			}
			else if (backGradientType != 0)
			{
				result = GetGradientBrush(rect, backColor, backGradientEndColor, backGradientType);
			}
			return result;
		}

		/// <summary>
		/// Interface-typed sibling of <see cref="CreateBrush"/> (Milestone B2 equivalent, dual-overload
		/// strategy). <see cref="CreateBrush"/> has exactly one real caller
		/// (<c>NumericIndicator.DrawBackground</c>), which only ever hands the result to
		/// <c>FillRectangle(IBrush, RectangleF)</c> (a value-type-only call, no concrete-GraphicsPath
		/// coupling) — migrated below. See tasks/gauge-gdi-type-abstraction.md Milestone B.
		/// </summary>
		internal IBrush CreateBrushResource(RectangleF rect, Color backColor, GaugeHatchStyle backHatchStyle, string backImage, GaugeImageWrapMode backImageMode, Color backImageTranspColor, GaugeImageAlign backImageAlign, GradientType backGradientType, Color backGradientEndColor)
		{
			if (backImage.Length > 0 && backImageMode != GaugeImageWrapMode.Unscaled && backImageMode != GaugeImageWrapMode.Scaled)
			{
				return GetTextureBrushResource(backImage, backImageTranspColor, backImageMode);
			}
			if (backHatchStyle != 0)
			{
				return GetHatchBrushResource(backHatchStyle, backColor, backGradientEndColor);
			}
			if (backGradientType != 0)
			{
				return GetGradientBrushResource(rect, backColor, backGradientEndColor, backGradientType);
			}
			return ResourceFactory.CreateSolidBrush(backColor);
		}

		public PointF PixelsToPercents(PointF pointInPixels)
		{
			return GetRelativePoint(pointInPixels);
		}

		public PointF PercentsToPixels(PointF pointInPercents)
		{
			return GetAbsolutePoint(pointInPercents);
		}

		public SizeF PixelsToPercents(SizeF sizeInPixels)
		{
			return GetRelativeSize(sizeInPixels);
		}

		public SizeF PercentsToPixels(SizeF sizeInPercents)
		{
			return GetAbsoluteSize(sizeInPercents);
		}

		internal float GetRelativeX(float absoluteX)
		{
			return absoluteX * 100f / (width - 1f);
		}

		internal float GetRelativeY(float absoluteY)
		{
			return absoluteY * 100f / (height - 1f);
		}

		internal float GetRelativeWidth(float absoluteWidth)
		{
			return absoluteWidth * 100f / (width - 1f);
		}

		internal float GetRelativeHeight(float absoluteHeight)
		{
			return absoluteHeight * 100f / (height - 1f);
		}

		internal float GetAbsoluteX(float relativeX)
		{
			return relativeX * (width - 1f) / 100f;
		}

		internal float GetAbsoluteY(float relativeY)
		{
			return relativeY * (height - 1f) / 100f;
		}

		internal float GetAbsoluteWidth(float relativeWidth)
		{
			return relativeWidth * (width - 1f) / 100f;
		}

		internal float GetAbsoluteHeight(float relativeHeight)
		{
			return relativeHeight * (height - 1f) / 100f;
		}

		public RectangleF GetRelativeRectangle(RectangleF absolute)
		{
			RectangleF empty = RectangleF.Empty;
			empty.X = GetRelativeX(absolute.X);
			empty.Y = GetRelativeY(absolute.Y);
			empty.Width = GetRelativeWidth(absolute.Width);
			empty.Height = GetRelativeHeight(absolute.Height);
			return empty;
		}

		public PointF GetRelativePoint(PointF absolute)
		{
			PointF empty = PointF.Empty;
			empty.X = GetRelativeX(absolute.X);
			empty.Y = GetRelativeY(absolute.Y);
			return empty;
		}

		public SizeF GetRelativeSize(SizeF size)
		{
			SizeF empty = SizeF.Empty;
			empty.Width = GetRelativeWidth(size.Width);
			empty.Height = GetRelativeHeight(size.Height);
			return empty;
		}

		internal float GetAbsoluteDimension(float relative)
		{
			if (width < height)
			{
				return GetAbsoluteWidth(relative);
			}
			return GetAbsoluteHeight(relative);
		}

		internal float GetRelativeDimension(float absolute)
		{
			if (width < height)
			{
				return GetRelativeWidth(absolute);
			}
			return GetRelativeHeight(absolute);
		}

		public PointF GetAbsolutePoint(PointF relative)
		{
			PointF empty = PointF.Empty;
			empty.X = GetAbsoluteX(relative.X);
			empty.Y = GetAbsoluteY(relative.Y);
			return empty;
		}

		public RectangleF GetAbsoluteRectangle(RectangleF relative)
		{
			RectangleF empty = RectangleF.Empty;
			empty.X = GetAbsoluteX(relative.X);
			empty.Y = GetAbsoluteY(relative.Y);
			empty.Width = GetAbsoluteWidth(relative.Width);
			empty.Height = GetAbsoluteHeight(relative.Height);
			return empty;
		}

		public SizeF GetAbsoluteSize(SizeF relative)
		{
			SizeF empty = SizeF.Empty;
			empty.Width = GetAbsoluteWidth(relative.Width);
			empty.Height = GetAbsoluteHeight(relative.Height);
			return empty;
		}

		internal GraphicsPath CreateRoundedRectPath(RectangleF rect, float[] cornerRadius)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddLine(rect.X + cornerRadius[0], rect.Y, rect.Right - cornerRadius[1], rect.Y);
			graphicsPath.AddArc(rect.Right - 2f * cornerRadius[1], rect.Y, 2f * cornerRadius[1], 2f * cornerRadius[2], 270f, 90f);
			graphicsPath.AddLine(rect.Right, rect.Y + cornerRadius[2], rect.Right, rect.Bottom - cornerRadius[3]);
			graphicsPath.AddArc(rect.Right - 2f * cornerRadius[4], rect.Bottom - 2f * cornerRadius[3], 2f * cornerRadius[4], 2f * cornerRadius[3], 0f, 90f);
			graphicsPath.AddLine(rect.Right - cornerRadius[4], rect.Bottom, rect.X + cornerRadius[5], rect.Bottom);
			graphicsPath.AddArc(rect.X, rect.Bottom - 2f * cornerRadius[6], 2f * cornerRadius[5], 2f * cornerRadius[6], 90f, 90f);
			graphicsPath.AddLine(rect.X, rect.Bottom - cornerRadius[6], rect.X, rect.Y + cornerRadius[7]);
			graphicsPath.AddArc(rect.X, rect.Y, 2f * cornerRadius[0], 2f * cornerRadius[7], 180f, 90f);
			return graphicsPath;
		}

		internal Brush GetCircularRangeBrush(RectangleF rect, float startAngle, float sweepAngle, Color backColor, GaugeHatchStyle backHatchStyle, string backImage, GaugeImageWrapMode backImageMode, Color backImageTranspColor, GaugeImageAlign backImageAlign, RangeGradientType backGradientType, Color backGradientEndColor)
		{
			RectangleF absoluteRectangle = GetAbsoluteRectangle(rect);
			if (backHatchStyle != 0)
			{
				return GetHatchBrush(backHatchStyle, backColor, backGradientEndColor);
			}
			if (!backGradientEndColor.IsEmpty)
			{
				switch (backGradientType)
				{
				case RangeGradientType.Center:
					return GetPieGradientBrush(absoluteRectangle, backColor, backGradientEndColor);
				case RangeGradientType.StartToEnd:
				{
					using (GraphicsPath graphicsPath2 = new GraphicsPath())
					{
						graphicsPath2.AddPie(absoluteRectangle.X - 1f, absoluteRectangle.Y - 1f, absoluteRectangle.Width + 2f, absoluteRectangle.Height + 2f, startAngle - 1f, sweepAngle + 1f);
						graphicsPath2.Flatten(null, 0.3f);
						return new PathGradientBrush(graphicsPath2)
						{
							SurroundColors = GetSurroundColors(backColor, backGradientEndColor, graphicsPath2.PointCount),
							CenterColor = GetSurroundColors(backColor, backGradientEndColor, 3)[1],
							CenterPoint = new PointF(absoluteRectangle.X + absoluteRectangle.Width / 2f, absoluteRectangle.Y + absoluteRectangle.Height / 2f)
						};
					}
				}
				default:
				{
					using (GraphicsPath graphicsPath = new GraphicsPath())
					{
						graphicsPath.AddPie(absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle, sweepAngle);
						return GetGradientBrush(graphicsPath.GetBounds(), backColor, backGradientEndColor, (GradientType)Enum.Parse(typeof(GradientType), backGradientType.ToString()));
					}
				}
				case RangeGradientType.None:
					break;
				}
			}
			if (backImage.Length > 0 && backImageMode != GaugeImageWrapMode.Unscaled && backImageMode != GaugeImageWrapMode.Scaled)
			{
				return GetTextureBrush(backImage, backImageTranspColor, backImageMode);
			}
			return new SolidBrush(backColor);
		}

		/// <summary>
		/// Interface-typed sibling of <see cref="GetCircularRangeBrush"/> (dual-overload, additive). The
		/// <c>StartToEnd</c> branch's per-vertex <c>SurroundColors</c> array needed the path's post-flatten
		/// point count, unblocked by <see cref="IGraphicsPath.Flatten(float)"/> (added to close this gap —
		/// GDI+'s tolerance overload had no equivalent before).
		/// </summary>
		internal IBrush GetCircularRangeBrushResource(RectangleF rect, float startAngle, float sweepAngle, Color backColor, GaugeHatchStyle backHatchStyle, string backImage, GaugeImageWrapMode backImageMode, Color backImageTranspColor, GaugeImageAlign backImageAlign, RangeGradientType backGradientType, Color backGradientEndColor)
		{
			RectangleF absoluteRectangle = GetAbsoluteRectangle(rect);
			if (backHatchStyle != 0)
			{
				return GetHatchBrushResource(backHatchStyle, backColor, backGradientEndColor);
			}
			if (!backGradientEndColor.IsEmpty)
			{
				switch (backGradientType)
				{
				case RangeGradientType.Center:
					return GetPieGradientBrushResource(absoluteRectangle, backColor, backGradientEndColor);
				case RangeGradientType.StartToEnd:
				{
					using IGraphicsPath graphicsPath2 = ResourceFactory.CreatePath();
					graphicsPath2.AddPie(absoluteRectangle.X - 1f, absoluteRectangle.Y - 1f, absoluteRectangle.Width + 2f, absoluteRectangle.Height + 2f, startAngle - 1f, sweepAngle + 1f);
					graphicsPath2.Flatten(0.3f);
					IPathGradientBrush pathGradientBrush = ResourceFactory.CreatePathGradientBrush(graphicsPath2);
					pathGradientBrush.SurroundColors = GetSurroundColors(backColor, backGradientEndColor, graphicsPath2.PointCount);
					pathGradientBrush.CenterColor = GetSurroundColors(backColor, backGradientEndColor, 3)[1];
					pathGradientBrush.CenterPoint = new PointF(absoluteRectangle.X + absoluteRectangle.Width / 2f, absoluteRectangle.Y + absoluteRectangle.Height / 2f);
					return pathGradientBrush;
				}
				default:
				{
					using IGraphicsPath graphicsPath = ResourceFactory.CreatePath();
					graphicsPath.AddPie(absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle, sweepAngle);
					return GetGradientBrushResource(graphicsPath.GetBounds(), backColor, backGradientEndColor, (GradientType)Enum.Parse(typeof(GradientType), backGradientType.ToString()));
				}
				case RangeGradientType.None:
					break;
				}
			}
			if (backImage.Length > 0 && backImageMode != GaugeImageWrapMode.Unscaled && backImageMode != GaugeImageWrapMode.Scaled)
			{
				return GetTextureBrushResource(backImage, backImageTranspColor, backImageMode);
			}
			return ResourceFactory.CreateSolidBrush(backColor);
		}

		internal IGraphicsPath GetCircularRangePath(RectangleF rect, float startAngle, float sweepAngle, float startRadius, float endRadius, Placement placement)
		{
			if (rect.Width == 0f || rect.Height == 0f)
			{
				return null;
			}
			IGraphicsPath graphicsPath = ResourceFactory.CreatePath();
			RectangleF absoluteRectangle = GetAbsoluteRectangle(rect);
			float num = GetAbsoluteDimension(startRadius);
			float num2 = GetAbsoluteDimension(endRadius);
			if (placement == Placement.Outside)
			{
				num = 0f - num;
				num2 = 0f - num2;
			}
			float num3 = (!(absoluteRectangle.Width > absoluteRectangle.Height)) ? (absoluteRectangle.Width / 2f - 0.0001f) : (absoluteRectangle.Height / 2f - 0.0001f);
			if (num > num3)
			{
				num = num3;
			}
			if (num2 > num3)
			{
				num2 = num3;
			}
			if (Math.Round(num - num2, 4) == 0.0)
			{
				if (placement == Placement.Cross)
				{
					absoluteRectangle.Inflate(num / 2f, num / 2f);
				}
				if (absoluteRectangle.Width > 0f && absoluteRectangle.Height > 0f)
				{
					graphicsPath.AddArc(absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle + sweepAngle, 0f - sweepAngle);
				}
				float num4 = absoluteRectangle.Width - num * 2f;
				float num5 = absoluteRectangle.Height - num * 2f;
				if (num4 > 0f && num5 > 0f)
				{
					graphicsPath.AddArc(absoluteRectangle.X + num, absoluteRectangle.Y + num, num4, num5, startAngle, sweepAngle);
				}
			}
			else if (placement != Placement.Cross)
			{
				graphicsPath.AddArc(absoluteRectangle.X, absoluteRectangle.Y, absoluteRectangle.Width, absoluteRectangle.Height, startAngle + sweepAngle, 0f - sweepAngle);
				int num6 = (int)(Math.Abs(sweepAngle) / 5f) + 1;
				if (num6 < 5)
				{
					num6 = 5;
				}
				float num7 = startAngle;
				float num8 = sweepAngle / (float)(num6 - 1);
				PointF[] array = new PointF[num6];
				PointF[] array2 = new PointF[1];
				PointF point = new PointF(absoluteRectangle.X + absoluteRectangle.Width / 2f, absoluteRectangle.Y + absoluteRectangle.Height / 2f);
				using (Matrix matrix = new Matrix())
				{
					for (int i = 0; i < num6; i++)
					{
						array2[0].X = point.X;
						array2[0].Y = absoluteRectangle.Y + absoluteRectangle.Height - num;
						array2[0].Y -= (num2 - num) * (float)i / (float)num6;
						matrix.RotateAt(num7 - 90f, point);
						matrix.TransformPoints(array2);
						matrix.Reset();
						array[i] = array2[0];
						num7 += num8;
					}
				}
				graphicsPath.AddCurve(array, 0.5f);
			}
			else
			{
				int num9 = (int)(Math.Abs(sweepAngle) / 5f) + 1;
				if (num9 < 5)
				{
					num9 = 5;
				}
				float num10 = startAngle;
				float num11 = sweepAngle / (float)(num9 - 1);
				PointF[] array3 = new PointF[num9];
				PointF[] array4 = new PointF[num9];
				PointF[] array5 = new PointF[1];
				PointF[] array6 = new PointF[1];
				PointF point2 = new PointF(absoluteRectangle.X + absoluteRectangle.Width / 2f, absoluteRectangle.Y + absoluteRectangle.Height / 2f);
				using (Matrix matrix2 = new Matrix())
				{
					for (int j = 0; j < num9; j++)
					{
						array5[0].X = point2.X;
						array5[0].Y = absoluteRectangle.Y + absoluteRectangle.Height - num / 2f;
						array5[0].Y -= (num2 - num) * (float)j / (float)num9 / 2f;
						array6[0].X = point2.X;
						array6[0].Y = absoluteRectangle.Y + absoluteRectangle.Height + num / 2f;
						array6[0].Y += (num2 - num) * (float)j / (float)num9 / 2f;
						matrix2.RotateAt(num10 - 90f, point2);
						matrix2.TransformPoints(array5);
						matrix2.TransformPoints(array6);
						matrix2.Reset();
						array3[j] = array5[0];
						array4[j] = array6[0];
						num10 += num11;
					}
				}
				PointF[] array7 = new PointF[num9];
				for (int k = 0; k < num9; k++)
				{
					array7[k] = array4[num9 - k - 1];
				}
				graphicsPath.AddCurve(array3, 0.5f);
				graphicsPath.AddCurve(array7, 0.5f);
			}
			graphicsPath.CloseFigure();
			return graphicsPath;
		}

		internal Color[] GetSurroundColors(Color startColor, Color endColor, int colorCount)
		{
			Color[] array = new Color[colorCount];
			array[0] = startColor;
			array[colorCount - 1] = endColor;
			float num = (endColor.A - startColor.A) / (colorCount - 1);
			float num2 = (endColor.R - startColor.R) / (colorCount - 1);
			float num3 = (endColor.G - startColor.G) / (colorCount - 1);
			float num4 = (endColor.B - startColor.B) / (colorCount - 1);
			float num5 = (int)startColor.A;
			float num6 = (int)startColor.R;
			float num7 = (int)startColor.G;
			float num8 = (int)startColor.B;
			for (int i = 1; i < colorCount - 1; i++)
			{
				num5 += num;
				num6 += num2;
				num7 += num3;
				num8 += num4;
				array[i] = Color.FromArgb((int)num5, (int)num6, (int)num7, (int)num8);
			}
			return array;
		}

		internal IGraphicsPath GetLinearRangePath(float startPosition, float endPosition, float startWidth, float endWidth, float scalePosition, GaugeOrientation orientation, float distanceFromScale, Placement placement, float scaleBarWidth)
		{
			PointF[] array = new PointF[4];
			array[0].X = endPosition;
			array[1].X = startPosition;
			array[2].X = startPosition;
			array[3].X = endPosition;
			switch (placement)
			{
			case Placement.Cross:
				array[0].Y = scalePosition + endWidth / 2f - distanceFromScale;
				array[1].Y = scalePosition + startWidth / 2f - distanceFromScale;
				array[2].Y = scalePosition - startWidth / 2f - distanceFromScale;
				array[3].Y = scalePosition - endWidth / 2f - distanceFromScale;
				break;
			case Placement.Inside:
				array[0].Y = scalePosition - scaleBarWidth / 2f - distanceFromScale;
				array[1].Y = array[0].Y;
				array[2].Y = array[0].Y - startWidth;
				array[3].Y = array[1].Y - endWidth;
				break;
			default:
				array[0].Y = scalePosition + scaleBarWidth / 2f + distanceFromScale;
				array[1].Y = array[0].Y;
				array[2].Y = array[0].Y + startWidth;
				array[3].Y = array[1].Y + endWidth;
				break;
			}
			if (orientation == GaugeOrientation.Vertical)
			{
				for (int i = 0; i < 4; i++)
				{
					float x = array[i].X;
					array[i].X = array[i].Y;
					array[i].Y = x;
				}
			}
			for (int j = 0; j < 4; j++)
			{
				array[j] = GetAbsolutePoint(array[j]);
			}
			IGraphicsPath graphicsPath = ResourceFactory.CreatePath();
			graphicsPath.AddLines(array);
			graphicsPath.CloseFigure();
			return graphicsPath;
		}

		internal Brush GetLinearRangeBrush(RectangleF absRect, Color backColor, GaugeHatchStyle backHatchStyle, RangeGradientType backGradientType, Color backGradientEndColor, GaugeOrientation orientation, bool reversedScale, double startValue, double endValue)
		{
			if (backHatchStyle != 0)
			{
				return GetHatchBrush(backHatchStyle, backColor, backGradientEndColor);
			}
			if (backGradientType != 0)
			{
				if (backGradientType == RangeGradientType.StartToEnd)
				{
					if (orientation == GaugeOrientation.Horizontal)
					{
						backGradientType = RangeGradientType.LeftRight;
					}
					else
					{
						backGradientType = RangeGradientType.TopBottom;
						Color color = backColor;
						backColor = backGradientEndColor;
						backGradientEndColor = color;
					}
					if (startValue > endValue)
					{
						Color color2 = backColor;
						backColor = backGradientEndColor;
						backGradientEndColor = color2;
					}
					if (reversedScale)
					{
						Color color3 = backColor;
						backColor = backGradientEndColor;
						backGradientEndColor = color3;
					}
				}
				return GetGradientBrush(absRect, backColor, backGradientEndColor, (GradientType)Enum.Parse(typeof(GradientType), backGradientType.ToString()));
			}
			return new SolidBrush(backColor);
		}

		/// <summary>Interface-typed sibling of <see cref="GetLinearRangeBrush"/> (dual-overload, additive). Fully self-contained — no gap.</summary>
		internal IBrush GetLinearRangeBrushResource(RectangleF absRect, Color backColor, GaugeHatchStyle backHatchStyle, RangeGradientType backGradientType, Color backGradientEndColor, GaugeOrientation orientation, bool reversedScale, double startValue, double endValue)
		{
			if (backHatchStyle != 0)
			{
				return GetHatchBrushResource(backHatchStyle, backColor, backGradientEndColor);
			}
			if (backGradientType != 0)
			{
				if (backGradientType == RangeGradientType.StartToEnd)
				{
					if (orientation == GaugeOrientation.Horizontal)
					{
						backGradientType = RangeGradientType.LeftRight;
					}
					else
					{
						backGradientType = RangeGradientType.TopBottom;
						Color color = backColor;
						backColor = backGradientEndColor;
						backGradientEndColor = color;
					}
					if (startValue > endValue)
					{
						Color color2 = backColor;
						backColor = backGradientEndColor;
						backGradientEndColor = color2;
					}
					if (reversedScale)
					{
						Color color3 = backColor;
						backColor = backGradientEndColor;
						backGradientEndColor = color3;
					}
				}
				return GetGradientBrushResource(absRect, backColor, backGradientEndColor, (GradientType)Enum.Parse(typeof(GradientType), backGradientType.ToString()));
			}
			return ResourceFactory.CreateSolidBrush(backColor);
		}

		internal IGraphicsPath GetThermometerPath(float startPosition, float endPosition, float barWidth, float scalePosition, GaugeOrientation orientation, float distanceFromScale, Placement placement, bool reversedScale, float scaleBarWidth, float bulbOffset, float bulbSize, ThermometerStyle thermometerStyle)
		{
			PointF[] array = new PointF[4];
			array[0].X = endPosition;
			array[1].X = startPosition;
			array[2].X = startPosition;
			array[3].X = endPosition;
			switch (placement)
			{
			case Placement.Cross:
				array[0].Y = scalePosition + barWidth / 2f - distanceFromScale;
				array[1].Y = scalePosition + barWidth / 2f - distanceFromScale;
				array[2].Y = scalePosition - barWidth / 2f - distanceFromScale;
				array[3].Y = scalePosition - barWidth / 2f - distanceFromScale;
				break;
			case Placement.Inside:
				array[0].Y = scalePosition - scaleBarWidth / 2f - distanceFromScale;
				array[1].Y = array[0].Y;
				array[2].Y = array[0].Y - barWidth;
				array[3].Y = array[1].Y - barWidth;
				break;
			default:
				array[0].Y = scalePosition + scaleBarWidth / 2f + distanceFromScale;
				array[1].Y = array[0].Y;
				array[2].Y = array[0].Y + barWidth;
				array[3].Y = array[1].Y + barWidth;
				break;
			}
			if (orientation == GaugeOrientation.Vertical)
			{
				for (int i = 0; i < 4; i++)
				{
					float x = array[i].X;
					array[i].X = array[i].Y;
					array[i].Y = x;
				}
			}
			for (int j = 0; j < 4; j++)
			{
				array[j] = GetAbsolutePoint(array[j]);
			}
			IGraphicsPath graphicsPath = ResourceFactory.CreatePath();
			graphicsPath.AddLines(array);
			if (bulbSize > 0f)
			{
				float absoluteDimension = GetAbsoluteDimension(bulbOffset);
				float num = GetAbsoluteDimension(bulbSize) / 2f;
				RectangleF bounds = graphicsPath.GetBounds();
				graphicsPath.Reset();
				PointF point = new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height / 2f);
				if (orientation == GaugeOrientation.Horizontal)
				{
					bounds.Offset(0f - absoluteDimension, 0f);
					bounds.Width += absoluteDimension;
					PointF pointF = new PointF(bounds.X - num, bounds.Y + bounds.Height / 2f);
					RectangleF rect = new RectangleF(pointF.X, pointF.Y, 0f, 0f);
					rect.Inflate(num, num);
					float num2 = 90f;
					if (rect.Height >= bounds.Height)
					{
						num2 = (float)Math.Asin(bounds.Height / 2f / num) * (180f / (float)Math.PI);
					}
					float sweepAngle = 360f - num2 * 2f;
					float x2 = num - num * (float)Math.Cos(num2 * (float)Math.PI / 180f);
					rect.Offset(x2, 0f);
					if (thermometerStyle == ThermometerStyle.Flask)
					{
						num2 = 90f;
						sweepAngle = 180f;
					}
					graphicsPath.AddArc(rect.X, rect.Y, rect.Width, rect.Height, num2, sweepAngle);
					graphicsPath.AddLine(bounds.Left, bounds.Top, bounds.Right, bounds.Top);
					graphicsPath.AddLine(bounds.Right, bounds.Top, bounds.Right, bounds.Bottom);
					graphicsPath.AddLine(bounds.Right, bounds.Bottom, bounds.Left, bounds.Bottom);
				}
				else
				{
					bounds.Height += absoluteDimension;
					PointF pointF2 = new PointF(bounds.X + bounds.Width / 2f, bounds.Y + bounds.Height + num);
					RectangleF rect2 = new RectangleF(pointF2.X, pointF2.Y, 0f, 0f);
					rect2.Inflate(num, num);
					float num3 = 90f;
					if (rect2.Width >= bounds.Width)
					{
						num3 = (float)Math.Asin(bounds.Width / 2f / num) * (180f / (float)Math.PI);
					}
					float sweepAngle2 = 360f - num3 * 2f;
					float num4 = num - num * (float)Math.Cos(num3 * (float)Math.PI / 180f);
					rect2.Offset(0f, 0f - num4);
					if (thermometerStyle == ThermometerStyle.Flask)
					{
						num3 = 90f;
						sweepAngle2 = 180f;
					}
					graphicsPath.AddArc(rect2.X, rect2.Y, rect2.Width, rect2.Height, num3 - 90f, sweepAngle2);
					graphicsPath.AddLine(bounds.Left, bounds.Bottom, bounds.Left, bounds.Top);
					graphicsPath.AddLine(bounds.Left, bounds.Top, bounds.Right, bounds.Top);
					graphicsPath.AddLine(bounds.Right, bounds.Top, bounds.Right, bounds.Bottom);
				}
				if (reversedScale)
				{
					using (Matrix matrix = new Matrix())
					{
						matrix.RotateAt(180f, point, MatrixOrder.Append);
						float[] elements = matrix.Elements;
						graphicsPath.Transform(new Matrix3x2(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]));
					}
				}
			}
			graphicsPath.CloseFigure();
			return graphicsPath;
		}

		/// <summary>
		/// Interface-typed replacement for the formerly-concrete <c>GetCircularEdgeReflection</c> (removed —
		/// dead code eliminated once <c>Knob</c>/<c>CircularPointer</c>'s reflection producers switched to
		/// this sibling during the atomic retyping pass).
		/// The path's final rotate+translate is a literal 1:1 port: the exact same native
		/// <see cref="Matrix"/>/<c>Rotate</c>/<c>Translate</c> sequence is built, then its element values are
		/// carried into a <see cref="Matrix3x2"/> for <see cref="IGraphicsPath.Transform(Matrix3x2)"/> — no
		/// re-derivation of rotation direction/order, same approach as <see cref="GetMarkerBrushResource"/>.
		/// </summary>
		internal void GetCircularEdgeReflectionResource(RectangleF bounds, float angle, int alpha, PointF pointOrigin, out IGraphicsPath pathResult, out IBrush brushResult)
		{
			pathResult = null;
			brushResult = null;
			if ((double)bounds.Width < 0.0001 || (double)bounds.Height < 0.0001)
			{
				return;
			}
			float num = 0.05f;
			float num2 = 0.05f;
			RectangleF rectangleF = bounds;
			rectangleF.Inflate((0f - bounds.Width) * num, (0f - bounds.Height) * num);
			RectangleF rect = rectangleF;
			rectangleF.Inflate((0f - rectangleF.Width) * num2, rectangleF.Height * num2);
			IGraphicsPath path = ResourceFactory.CreatePath();
			path.AddArc(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height, angle, 90f);
			path.AddArc(rect.X, rect.Y, rect.Width, rect.Height, angle + 90f, -90f);
			ILinearGradientBrush linearGradientBrush = ResourceFactory.CreateLinearGradientBrush(bounds, Color.Transparent, Color.FromArgb(alpha, Color.White), 0f);
			linearGradientBrush.Blend = new Blend
			{
				Positions = new float[5] { 0f, 0.1f, 0.5f, 0.9f, 1f },
				Factors = new float[5] { 0f, 0f, 1f, 0f, 0f }
			};
			linearGradientBrush.RotateTransform(135f, MatrixOrder.Append);
			linearGradientBrush.TranslateTransform(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
			brushResult = linearGradientBrush;
			using (Matrix matrix = new Matrix())
			{
				matrix.Rotate(45f, MatrixOrder.Append);
				matrix.Translate(pointOrigin.X, pointOrigin.Y, MatrixOrder.Append);
				float[] elements = matrix.Elements;
				path.Transform(new Matrix3x2(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]));
			}
			pathResult = path;
		}

		internal void SetPictureSize(float width, float height)
		{
			this.width = Math.Max(width, 2f);
			this.height = Math.Max(height, 2f);
		}

		internal void CreateDrawRegion(RectangleF rect)
		{
			graphicStates.Push(new GaugeGraphState(Save(), width, height));
			RectangleF absoluteRectangle = GetAbsoluteRectangle(rect);
			if (base.Transform == null)
			{
				base.Transform = new Matrix();
			}
			TranslateTransform(absoluteRectangle.Location.X, absoluteRectangle.Location.Y);
			SetPictureSize(absoluteRectangle.Size.Width, absoluteRectangle.Size.Height);
		}

		internal void RestoreDrawRegion()
		{
			GaugeGraphState gaugeGraphState = (GaugeGraphState)graphicStates.Pop();
			Restore(gaugeGraphState.state);
			SetPictureSize(gaugeGraphState.width, gaugeGraphState.height);
		}

		internal GaugeGraphics(CommonElements common)
		{
			this.common = common;
			common.Graph = this;
			pen = new Pen(Color.Black);
			solidBrush = new SolidBrush(Color.Black);
		}

		public override void Close()
		{
			common.Graph = null;
			base.Close();
		}

		internal Pen GetSelectionPen(bool designTimeSelection, Color borderColor)
		{
			Pen pen = null;
			if (designTimeSelection)
			{
				pen = new Pen(Color.Black, 1f);
				pen.DashStyle = DashStyle.Dot;
				pen.DashPattern = new float[2]
				{
					2f,
					2f
				};
				pen.Width = 1f / Graphics.PageScale;
			}
			else
			{
				pen = new Pen(borderColor, 1f);
				pen.DashStyle = DashStyle.Dot;
				pen.DashPattern = new float[2]
				{
					2f,
					2f
				};
			}
			return pen;
		}

		/// <summary>Milestone B2 equivalent — self-contained (no shared field/concrete-path coupling), fully converted to the interface-typed resource surface. See tasks/gauge-gdi-type-abstraction.md.</summary>
		internal IBrush GetDesignTimeSelectionFillBrush()
		{
			return ResourceFactory.CreateSolidBrush(Color.White);
		}

		/// <summary>Milestone B2 equivalent — see <see cref="GetDesignTimeSelectionFillBrush"/>.</summary>
		internal IPen GetDesignTimeSelectionBorderPen()
		{
			return ResourceFactory.CreatePen(Color.Black, 1f / Graphics.PageScale);
		}

		/// <summary>
		/// Interface-typed sibling of <see cref="GetSelectionPen"/> (Milestone B2 equivalent, dual-overload
		/// strategy per tasks/gauge-gdi-type-abstraction.md). Used wherever the resulting pen is only ever
		/// handed to an interface-typed drawing call (e.g. <see cref="DrawSelection"/>'s rectangle border) —
		/// <see cref="DrawRadialSelection"/> still needs the original concrete-returning method, since it draws
		/// onto a concrete <see cref="GraphicsPath"/> and no mixed <c>DrawPath(IPen, GraphicsPath)</c> overload exists.
		/// </summary>
		internal IPen GetSelectionPenResource(bool designTimeSelection, Color borderColor)
		{
			IPen pen;
			if (designTimeSelection)
			{
				pen = ResourceFactory.CreatePen(Color.Black, 1f);
				pen.DashStyle = DashStyle.Dot;
				pen.DashPattern = new float[2]
				{
					2f,
					2f
				};
				pen.Width = 1f / Graphics.PageScale;
			}
			else
			{
				pen = ResourceFactory.CreatePen(borderColor, 1f);
				pen.DashStyle = DashStyle.Dot;
				pen.DashPattern = new float[2]
				{
					2f,
					2f
				};
			}
			return pen;
		}

		internal void DrawSelection(RectangleF rect, bool designTimeSelection, Color borderColor, Color markerColor)
		{
			DrawSelection(rect, 3f / Graphics.PageScale, designTimeSelection, borderColor, markerColor);
		}

		internal void DrawSelection(RectangleF rect, float inflateBy, bool designTimeSelection, Color borderColor, Color markerColor)
		{
			float num = 20f;
			rect.Inflate(inflateBy, inflateBy);
			rect = RectangleF.Intersect(rect, Graphics.VisibleClipBounds);
			PointF pointF = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
			float num2 = 6f / Graphics.PageScale;
			using (IPen pen = GetSelectionPenResource(designTimeSelection, borderColor))
			{
				DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
			}
			PointF[] array = new PointF[8];
			array[0].X = rect.X;
			array[0].Y = rect.Y;
			array[1].X = pointF.X;
			array[1].Y = rect.Y;
			array[2].X = rect.X + rect.Width;
			array[2].Y = rect.Y;
			array[3].X = rect.X;
			array[3].Y = pointF.Y;
			array[4].X = rect.X + rect.Width;
			array[4].Y = pointF.Y;
			array[5].X = rect.X;
			array[5].Y = rect.Y + rect.Height;
			array[6].X = pointF.X;
			array[6].Y = rect.Y + rect.Height;
			array[7].X = rect.X + rect.Width;
			array[7].Y = rect.Y + rect.Height;
			IBrush brush = null;
			IPen pen2 = null;
			if (designTimeSelection)
			{
				brush = GetDesignTimeSelectionFillBrush();
				pen2 = GetDesignTimeSelectionBorderPen();
			}
			else
			{
				brush = ResourceFactory.CreateSolidBrush(markerColor);
				pen2 = ResourceFactory.CreatePen(borderColor, 1f);
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (((i != 1 && i != 6) || !(rect.Width < num)) && ((i != 3 && i != 4) || !(rect.Height < num)))
				{
					FillEllipse(brush, new RectangleF(array[i].X - num2 / 2f, array[i].Y - num2 / 2f, num2, num2));
					DrawEllipse(pen2, new RectangleF(array[i].X - num2 / 2f, array[i].Y - num2 / 2f, num2, num2));
				}
			}
			brush?.Dispose();
			pen2?.Dispose();
		}

		internal void DrawRadialSelection(GaugeGraphics g, GraphicsPath selectionPath, PointF[] markerPositions, bool designTimeSelection, Color borderColor, Color markerColor)
		{
			// selectionPath is a concrete GraphicsPath built by the caller (e.g. CircularScale.GetBarPath) and no
			// mixed DrawPath(IPen, GraphicsPath) overload exists, so this call stays on the original concrete
			// GetSelectionPen. See tasks/gauge-gdi-type-abstraction.md Milestone B2 notes.
			DrawPath(GetSelectionPen(designTimeSelection, borderColor), selectionPath);
			float num = 6f / Graphics.PageScale;
			IBrush brush = null;
			IPen pen = null;
			if (designTimeSelection)
			{
				brush = GetDesignTimeSelectionFillBrush();
				pen = GetDesignTimeSelectionBorderPen();
			}
			else
			{
				brush = ResourceFactory.CreateSolidBrush(markerColor);
				pen = ResourceFactory.CreatePen(borderColor, 1f);
			}
			for (int i = 0; i < markerPositions.Length; i++)
			{
				FillEllipse(brush, new RectangleF(markerPositions[i].X - num / 2f, markerPositions[i].Y - num / 2f, num, num));
				DrawEllipse(pen, new RectangleF(markerPositions[i].X - num / 2f, markerPositions[i].Y - num / 2f, num, num));
			}
			brush?.Dispose();
			pen?.Dispose();
		}
	}
}
