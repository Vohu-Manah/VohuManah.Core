using Reporting.Rendering;

namespace Reporting.Gauge.WebForms
{
	internal class KnobStyleAttrib
	{
		public IGraphicsPath[] paths;

		public IBrush[] brushes;

		public KnobStyleAttrib()
		{
			paths = null;
			brushes = null;
		}

		public void Dispose()
		{
			if (paths != null)
			{
				IGraphicsPath[] array = paths;
				for (int i = 0; i < array.Length; i++)
				{
					array[i]?.Dispose();
				}
				paths = null;
			}
			if (brushes != null)
			{
				IBrush[] array2 = brushes;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i]?.Dispose();
				}
				brushes = null;
			}
		}
	}
}
