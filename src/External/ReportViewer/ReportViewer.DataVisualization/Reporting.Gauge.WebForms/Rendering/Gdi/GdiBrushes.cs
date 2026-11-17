using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Numerics;
using Reporting.Rendering;

namespace Reporting.Gauge.WebForms.Rendering.Gdi
{
	/// <summary>Milestone A2 adapter — wraps <see cref="System.Drawing.SolidBrush"/> behind <see cref="ISolidBrush"/>.</summary>
	internal sealed class GdiSolidBrush : ISolidBrush
	{
		internal SolidBrush NativeBrush { get; }

		internal GdiSolidBrush(Color color)
		{
			NativeBrush = new SolidBrush(color);
		}

		public Color Color
		{
			get => NativeBrush.Color;
			set => NativeBrush.Color = value;
		}

		public ISolidBrush Clone() => new GdiSolidBrush((SolidBrush)NativeBrush.Clone());

		private GdiSolidBrush(SolidBrush nativeBrush)
		{
			NativeBrush = nativeBrush;
		}

		public void Dispose() => NativeBrush.Dispose();
	}

	/// <summary>Milestone A2 adapter — wraps <see cref="System.Drawing.Drawing2D.LinearGradientBrush"/> behind <see cref="ILinearGradientBrush"/>.</summary>
	internal sealed class GdiLinearGradientBrush : ILinearGradientBrush
	{
		internal LinearGradientBrush NativeBrush { get; }

		internal GdiLinearGradientBrush(RectangleF rect, Color startColor, Color endColor, float angle)
		{
			NativeBrush = new LinearGradientBrush(rect, startColor, endColor, angle);
		}

		internal GdiLinearGradientBrush(PointF point1, PointF point2, Color color1, Color color2)
		{
			NativeBrush = new LinearGradientBrush(point1, point2, color1, color2);
		}

		public Blend Blend
		{
			get => NativeBrush.Blend;
			set => NativeBrush.Blend = value;
		}

		public ColorBlend InterpolationColors
		{
			get => NativeBrush.InterpolationColors;
			set => NativeBrush.InterpolationColors = value;
		}

		public WrapMode WrapMode
		{
			get => NativeBrush.WrapMode;
			set => NativeBrush.WrapMode = value;
		}

		public Color[] LinearColors => NativeBrush.LinearColors;

		public void SetRotationTransform(float angle, PointF center)
		{
			using Matrix matrix = new Matrix();
			matrix.RotateAt(angle, center);
			NativeBrush.Transform = matrix;
		}

		public void RotateTransform(float angle, MatrixOrder order) => NativeBrush.RotateTransform(angle, order);

		public void TranslateTransform(float dx, float dy, MatrixOrder order) => NativeBrush.TranslateTransform(dx, dy, order);

		public void MultiplyTransform(Matrix3x2 matrix, MatrixOrder order)
		{
			using Matrix bridgedMatrix = new Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.M31, matrix.M32);
			using Matrix transform = NativeBrush.Transform.Clone();
			transform.Multiply(bridgedMatrix, order);
			NativeBrush.Transform = transform;
		}

		public void Dispose() => NativeBrush.Dispose();
	}

	/// <summary>Milestone A2 adapter — wraps <see cref="System.Drawing.TextureBrush"/> behind <see cref="ITextureBrush"/>.</summary>
	internal sealed class GdiTextureBrush : ITextureBrush
	{
		internal TextureBrush NativeBrush { get; }

		internal GdiTextureBrush(Image image, WrapMode wrapMode)
		{
			NativeBrush = new TextureBrush(image, wrapMode);
		}

		internal GdiTextureBrush(Image image, RectangleF rect, ImageAttributes attributes)
		{
			NativeBrush = new TextureBrush(image, rect, attributes);
		}

		public WrapMode WrapMode
		{
			get => NativeBrush.WrapMode;
			set => NativeBrush.WrapMode = value;
		}

		public void Dispose() => NativeBrush.Dispose();
	}

	/// <summary>Milestone A2 adapter — wraps <see cref="System.Drawing.Drawing2D.HatchBrush"/> behind <see cref="IHatchBrush"/>.</summary>
	internal sealed class GdiHatchBrush : IHatchBrush
	{
		internal HatchBrush NativeBrush { get; }

		internal GdiHatchBrush(HatchStyle style, Color foreColor, Color backColor)
		{
			NativeBrush = new HatchBrush(style, foreColor, backColor);
		}

		public HatchStyle HatchStyle => NativeBrush.HatchStyle;

		public Color ForegroundColor => NativeBrush.ForegroundColor;

		public Color BackgroundColor => NativeBrush.BackgroundColor;

		public void Dispose() => NativeBrush.Dispose();
	}

	/// <summary>Milestone A2 adapter — wraps <see cref="System.Drawing.Drawing2D.PathGradientBrush"/> behind <see cref="IPathGradientBrush"/>.</summary>
	internal sealed class GdiPathGradientBrush : IPathGradientBrush
	{
		internal PathGradientBrush NativeBrush { get; }

		internal GdiPathGradientBrush(IGraphicsPath path)
		{
			NativeBrush = new PathGradientBrush(((GdiGraphicsPath)path).NativePath);
		}

		public Color CenterColor
		{
			get => NativeBrush.CenterColor;
			set => NativeBrush.CenterColor = value;
		}

		public Color[] SurroundColors
		{
			get => NativeBrush.SurroundColors;
			set => NativeBrush.SurroundColors = value;
		}

		public PointF CenterPoint
		{
			get => NativeBrush.CenterPoint;
			set => NativeBrush.CenterPoint = value;
		}

		public PointF FocusScales
		{
			get => new PointF(NativeBrush.FocusScales.X, NativeBrush.FocusScales.Y);
			set => NativeBrush.FocusScales = value;
		}

		public Blend Blend
		{
			get => NativeBrush.Blend;
			set => NativeBrush.Blend = value;
		}

		public ColorBlend InterpolationColors
		{
			get => NativeBrush.InterpolationColors;
			set => NativeBrush.InterpolationColors = value;
		}

		public void SetRotationTransform(float angle, PointF center)
		{
			using Matrix matrix = new Matrix();
			matrix.RotateAt(angle, center);
			NativeBrush.Transform = matrix;
		}

		public void RotateTransform(float angle, MatrixOrder order) => NativeBrush.RotateTransform(angle, order);

		public void TranslateTransform(float dx, float dy, MatrixOrder order) => NativeBrush.TranslateTransform(dx, dy, order);

		public void MultiplyTransform(Matrix3x2 matrix, MatrixOrder order)
		{
			using Matrix bridgedMatrix = new Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.M31, matrix.M32);
			using Matrix transform = NativeBrush.Transform.Clone();
			transform.Multiply(bridgedMatrix, order);
			NativeBrush.Transform = transform;
		}

		public void Dispose() => NativeBrush.Dispose();
	}
}
