using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using Reporting.Rendering;

namespace Reporting.Chart.WebForms.Rendering
{
	/// <summary>
	/// Milestone C2 helpers. <see cref="Matrix3x2"/> has no RotateAt/Translate/TransformPoints
	/// instance members, so these mirror GDI+'s <see cref="Matrix"/> API shape (Appendix A.6
	/// of chart-gdi-type-abstraction.md) using the same composition order GDI+ uses by default
	/// (<see cref="MatrixOrder.Prepend"/>): the new operation is applied in the pre-existing
	/// matrix's local space, i.e. <c>result = op * matrix</c>. Verified against
	/// <see cref="System.Drawing.Drawing2D.Matrix"/> point-for-point (RotateAt, Translate) before
	/// converting any call site — see the Phase 0/C2 spike notes in chart-gdi-type-abstraction.md.
	/// </summary>
	internal static class Matrix3x2Extensions
	{
		public static Matrix3x2 RotateAt(this Matrix3x2 matrix, float angleDegrees, PointF point) =>
			Matrix3x2.CreateRotation(angleDegrees * (float)(System.Math.PI / 180.0), new Vector2(point.X, point.Y)) * matrix;

		public static Matrix3x2 Translate(this Matrix3x2 matrix, float dx, float dy) =>
			Matrix3x2.CreateTranslation(dx, dy) * matrix;

		public static Matrix3x2 Scale(this Matrix3x2 matrix, float sx, float sy) =>
			Matrix3x2.CreateScale(sx, sy) * matrix;

		/// <summary>In-place batch transform, mirroring <c>Matrix.TransformPoints(PointF[])</c>.</summary>
		public static void TransformPoints(this Matrix3x2 matrix, PointF[] points)
		{
			for (int i = 0; i < points.Length; i++)
			{
				Vector2 result = Vector2.Transform(new Vector2(points[i].X, points[i].Y), matrix);
				points[i] = new PointF(result.X, result.Y);
			}
		}

		/// <summary>
		/// Temporary bridge back to a concrete GDI+ <see cref="Matrix"/> for the call sites that feed a
		/// still-concrete <see cref="GraphicsPath"/>/<see cref="Region"/>'s own <c>.Transform(Matrix)</c>
		/// overload. Remove once C7 (<c>GraphicsPath</c> → <c>IGraphicsPath</c>) lands and those overloads
		/// accept <see cref="Matrix3x2"/> directly.
		/// </summary>
		public static Matrix ToGdiMatrix(this Matrix3x2 matrix) =>
			new Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.M31, matrix.M32);
	}
}
