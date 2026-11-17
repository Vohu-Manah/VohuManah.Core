using Reporting.Rendering;

namespace Reporting.Gauge.WebForms
{
	internal class BarStyleAttrib
	{
		public IGraphicsPath primaryPath;

		public IBrush primaryBrush;

		public IGraphicsPath[] secondaryPaths;

		public IBrush[] secondaryBrushes;

		public IGraphicsPath totalPath;

		public IBrush totalBrush;

		public BarStyleAttrib()
		{
			primaryPath = null;
			primaryBrush = null;
			secondaryPaths = null;
			secondaryBrushes = null;
			totalPath = null;
			totalBrush = null;
		}

		public void Dispose()
		{
			if (primaryPath != null)
			{
				primaryPath.Dispose();
				primaryPath = null;
			}
			if (primaryBrush != null)
			{
				primaryBrush.Dispose();
				primaryBrush = null;
			}
			if (secondaryPaths != null)
			{
				IGraphicsPath[] array = secondaryPaths;
				for (int i = 0; i < array.Length; i++)
				{
					array[i]?.Dispose();
				}
				secondaryPaths = null;
			}
			if (secondaryBrushes != null)
			{
				IBrush[] array2 = secondaryBrushes;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i]?.Dispose();
				}
				secondaryBrushes = null;
			}
			if (totalPath != null)
			{
				totalPath.Dispose();
				totalPath = null;
			}
			if (totalBrush != null)
			{
				totalBrush.Dispose();
				totalBrush = null;
			}
		}
	}
}
