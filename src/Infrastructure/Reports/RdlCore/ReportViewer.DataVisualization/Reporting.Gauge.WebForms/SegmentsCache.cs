using System;
using System.Collections;
using System.Drawing;
using System.Numerics;
using Reporting.Gauge.WebForms.Rendering;
using Reporting.Rendering;

namespace Reporting.Gauge.WebForms
{
	/// <summary>
	/// Converted alongside <c>DigitalSegment.cs</c> (Milestone B2 open item #7,
	/// tasks/gauge-gdi-type-abstraction.md). <see cref="IGraphicsPath"/> has no <c>Clone()</c> member (not
	/// in Appendix A.1's method list), so the cached-copy semantics that GDI+'s <c>GraphicsPath.Clone()</c>
	/// provided are reconstructed via <see cref="IGaugeDrawingResourceFactory.CreatePath(PointF[], byte[])"/>
	/// — feeding the cached path's own <c>PathPoints</c>/<c>PathTypes</c> back in builds an independent copy
	/// without adding a new interface member. The translate-only <c>matrix</c> field (GDI+'s <c>Matrix</c> is
	/// mutable, reused via <c>Reset()</c>/<c>Translate()</c>) is replaced by a fresh
	/// <see cref="Matrix3x2.CreateTranslation(float, float)"/> built on each call — <see cref="Matrix3x2"/> is
	/// a value type with no mutation methods, matching the pattern <c>GaugeGraphics.DrawPathShadowAbs</c>'s
	/// interface-typed sibling already established.
	/// </summary>
	internal class SegmentsCache
	{
		private Hashtable cacheTable = new Hashtable();

		private float size = -1f;

		internal IGraphicsPath GetSegment(IGaugeDrawingResourceFactory resourceFactory, Enum segments, PointF p, float size)
		{
			CheckCache(size);
			if (cacheTable.Contains(segments))
			{
				IGraphicsPath cached = (IGraphicsPath)cacheTable[segments];
				IGraphicsPath obj = resourceFactory.CreatePath(cached.PathPoints, cached.PathTypes);
				obj.Transform(Matrix3x2.CreateTranslation(p.X, p.Y));
				return obj;
			}
			return null;
		}

		internal void Reset()
		{
			foreach (object value in cacheTable.Values)
			{
				if (value is IDisposable)
				{
					((IDisposable)value).Dispose();
				}
			}
			cacheTable.Clear();
		}

		private void CheckCache(float size)
		{
			if (Math.Abs(this.size - size) > float.Epsilon)
			{
				Reset();
				this.size = size;
			}
		}

		internal void SetSegment(IGaugeDrawingResourceFactory resourceFactory, Enum segment, IGraphicsPath path, PointF p, float size)
		{
			CheckCache(size);
			IGraphicsPath graphicsPath = resourceFactory.CreatePath(path.PathPoints, path.PathTypes);
			graphicsPath.Transform(Matrix3x2.CreateTranslation(0f - p.X, 0f - p.Y));
			cacheTable[segment] = graphicsPath;
		}
	}
}
