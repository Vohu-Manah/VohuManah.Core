using Reporting.Rendering;

namespace Reporting.Gauge.WebForms
{
	internal class MarkerStyleAttrib
	{
		public IGraphicsPath path;

		public IBrush brush;

		public MarkerStyleAttrib()
		{
			path = null;
			brush = null;
		}

		public void Dispose()
		{
			if (path != null)
			{
				path.Dispose();
			}
			if (brush != null)
			{
				brush.Dispose();
			}
		}
	}
}
