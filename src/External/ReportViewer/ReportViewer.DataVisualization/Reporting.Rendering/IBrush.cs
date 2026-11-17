using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;

namespace Reporting.Rendering
{
	/// <summary>
	/// Fill resource. Abstracts the <see cref="System.Drawing.Brush"/> hierarchy.
	/// Appendix A.5: <see cref="ISolidBrush"/> is ~90% of usage; the others are low-frequency.
	/// </summary>
	internal interface IBrush : IRenderingResource
	{
	}

	/// <summary>Solid single-colour fill. Abstracts <c>SolidBrush</c>.</summary>
	internal interface ISolidBrush : IBrush
	{
		Color Color { get; set; }

		/// <summary>Abstracts <c>SolidBrush.Clone()</c> — returns an independent copy with the same colour (found during the Gauge engine's <c>XamlLayer.SetSingleBrush</c> conversion; see tasks/gauge-gdi-type-abstraction.md item 2).</summary>
		ISolidBrush Clone();
	}

	/// <summary>Two-colour / multi-stop gradient fill. Abstracts <c>LinearGradientBrush</c>.</summary>
	internal interface ILinearGradientBrush : IBrush
	{
		Blend Blend { get; set; }

		ColorBlend InterpolationColors { get; set; }

		WrapMode WrapMode { get; set; }

		/// <summary>The 2-element [start, end] colour pair (GDI+'s <c>LinearColors</c>) — read back by <c>GetSector3DBrush</c>.</summary>
		Color[] LinearColors { get; }

		/// <summary>
		/// Replace the brush's transform with a rotation of <paramref name="angle"/> degrees around
		/// <paramref name="center"/> — GDI+'s <c>Matrix.RotateAt(angle, center)</c> assigned to
		/// <c>Brush.Transform</c> (found in the Gauge engine's marker/frame gradient-brush code, which
		/// builds this exact matrix and assigns it wholesale rather than composing with an existing
		/// transform; see tasks/gauge-gdi-type-abstraction.md Milestone B2).
		/// </summary>
		void SetRotationTransform(float angle, PointF center);

		/// <summary>Compose a rotation into the brush's existing transform — GDI+'s <c>RotateTransform(angle, order)</c>.</summary>
		void RotateTransform(float angle, MatrixOrder order);

		/// <summary>Compose a translation into the brush's existing transform — GDI+'s <c>TranslateTransform(dx, dy, order)</c>.</summary>
		void TranslateTransform(float dx, float dy, MatrixOrder order);

		/// <summary>
		/// Compose an arbitrary transform (scale/shear/general matrix) into the brush's existing
		/// transform — GDI+'s <c>Matrix.Multiply(matrix, order)</c> applied to <c>Brush.Transform</c>.
		/// Needed by the Gauge engine's <c>XamlRenderer.ApplyTransform</c>/<c>ApplyRelativeTransform</c>,
		/// whose XAML-declared brush transforms can compose scale/shear/general-matrix operations that
		/// <see cref="SetRotationTransform"/>/<see cref="RotateTransform"/>/<see cref="TranslateTransform"/>
		/// can't express (see tasks/gauge-gdi-type-abstraction.md item 2).
		/// </summary>
		void MultiplyTransform(Matrix3x2 matrix, MatrixOrder order);
	}

	/// <summary>Image-tiled fill. Abstracts <c>TextureBrush</c>.</summary>
	internal interface ITextureBrush : IBrush
	{
		WrapMode WrapMode { get; set; }
	}

	/// <summary>Hatch-pattern fill. Abstracts <c>HatchBrush</c> (rare — 1 construction site).</summary>
	internal interface IHatchBrush : IBrush
	{
		HatchStyle HatchStyle { get; }

		Color ForegroundColor { get; }

		Color BackgroundColor { get; }
	}

	/// <summary>
	/// Path-bounded gradient fill (soft shadows/glows). Abstracts <c>PathGradientBrush</c> —
	/// missed by the original Appendix A.5 count (16 occ / 2 files, all in the shadow-rendering
	/// code paths of ChartGraphics/ChartGraphics3D), discovered during C4.
	/// </summary>
	internal interface IPathGradientBrush : IBrush
	{
		Color CenterColor { get; set; }

		Color[] SurroundColors { get; set; }

		PointF CenterPoint { get; set; }

		PointF FocusScales { get; set; }

		Blend Blend { get; set; }

		ColorBlend InterpolationColors { get; set; }

		/// <summary>See <see cref="ILinearGradientBrush.SetRotationTransform"/> — identical role, GDI+'s <c>PathGradientBrush</c> implements the same <c>ITransform</c> shape.</summary>
		void SetRotationTransform(float angle, PointF center);

		/// <summary>See <see cref="ILinearGradientBrush.RotateTransform"/>.</summary>
		void RotateTransform(float angle, MatrixOrder order);

		/// <summary>See <see cref="ILinearGradientBrush.TranslateTransform"/>.</summary>
		void TranslateTransform(float dx, float dy, MatrixOrder order);

		/// <summary>See <see cref="ILinearGradientBrush.MultiplyTransform"/>.</summary>
		void MultiplyTransform(Matrix3x2 matrix, MatrixOrder order);
	}
}
