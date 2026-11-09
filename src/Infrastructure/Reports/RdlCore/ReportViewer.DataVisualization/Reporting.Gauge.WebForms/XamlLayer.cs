using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Reporting.Rendering;

namespace Reporting.Gauge.WebForms
{
	internal class XamlLayer : IDisposable
	{
		private IGraphicsPath[] paths;

		private IBrush[] brushes;

		private IPen[] pens;

		private XamlLayer[] innerLayers;

		private bool disposed;

		public IGraphicsPath[] Paths
		{
			get
			{
				return paths;
			}
			set
			{
				if (paths != value && paths != null)
				{
					IGraphicsPath[] array = paths;
					for (int i = 0; i < array.Length; i++)
					{
						array[i]?.Dispose();
					}
				}
				paths = value;
			}
		}

		public IBrush[] Brushes
		{
			get
			{
				return brushes;
			}
			set
			{
				if (brushes != value && brushes != null)
				{
					IBrush[] array = brushes;
					for (int i = 0; i < array.Length; i++)
					{
						array[i]?.Dispose();
					}
				}
				brushes = value;
			}
		}

		public IPen[] Pens
		{
			get
			{
				return pens;
			}
			set
			{
				if (pens != value && pens != null)
				{
					IPen[] array = pens;
					for (int i = 0; i < array.Length; i++)
					{
						array[i]?.Dispose();
					}
				}
				pens = value;
			}
		}

		public XamlLayer[] InnerLayers
		{
			get
			{
				return innerLayers;
			}
			set
			{
				if (innerLayers != value && innerLayers != null)
				{
					XamlLayer[] array = innerLayers;
					for (int i = 0; i < array.Length; i++)
					{
						array[i]?.Dispose();
					}
				}
				innerLayers = value;
			}
		}

		public void Render(GaugeGraphics g)
		{
			if (InnerLayers != null)
			{
				for (int i = 0; i < InnerLayers.Length; i++)
				{
					InnerLayers[i].Render(g);
				}
			}
			if (Paths == null)
			{
				return;
			}
			for (int j = 0; j < Paths.Length; j++)
			{
				if (Brushes[j] != null)
				{
					g.FillPath(Brushes[j], Paths[j]);
				}
				if (Pens[j] != null)
				{
					g.DrawPath(Pens[j], Paths[j]);
				}
			}
		}

		public void SetSingleBrush(ISolidBrush brush)
		{
			Brushes = new IBrush[Paths.Length];
			for (int i = 0; i < Brushes.Length; i++)
			{
				Brushes[i] = brush.Clone();
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed && disposing)
			{
				Paths = null;
				Brushes = null;
				Pens = null;
				InnerLayers = null;
			}
			disposed = true;
		}

		~XamlLayer()
		{
			Dispose(disposing: false);
		}
	}
}
