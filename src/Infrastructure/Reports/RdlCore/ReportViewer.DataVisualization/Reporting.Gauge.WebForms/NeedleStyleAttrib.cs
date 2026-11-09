using Reporting.Rendering;

namespace Reporting.Gauge.WebForms
{
	internal class NeedleStyleAttrib
	{
		public IGraphicsPath primaryPath;

		public IGraphicsPath secondaryPath;

		public IBrush primaryBrush;

		public IBrush secondaryBrush;

		public IGraphicsPath[] reflectionPaths;

		public IBrush[] reflectionBrushes;

		public NeedleStyleAttrib()
		{
			primaryPath = null;
			secondaryPath = null;
			primaryBrush = null;
			secondaryBrush = null;
			reflectionPaths = null;
			reflectionBrushes = null;
		}

		public void Dispose()
		{
			if (primaryPath != null)
			{
				primaryPath.Dispose();
				primaryPath = null;
			}
			if (secondaryPath != null)
			{
				secondaryPath.Dispose();
				secondaryPath = null;
			}
			if (primaryBrush != null)
			{
				primaryBrush.Dispose();
				primaryBrush = null;
			}
			if (secondaryBrush != null)
			{
				secondaryBrush.Dispose();
				secondaryBrush = null;
			}
			if (reflectionPaths != null)
			{
				IGraphicsPath[] array = reflectionPaths;
				for (int i = 0; i < array.Length; i++)
				{
					array[i]?.Dispose();
				}
				reflectionPaths = null;
			}
			if (reflectionBrushes != null)
			{
				IBrush[] array2 = reflectionBrushes;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i]?.Dispose();
				}
				reflectionBrushes = null;
			}
		}
	}
}
