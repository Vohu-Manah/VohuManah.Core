using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Resources;

namespace Reporting.Chart.WebForms.Utilities
{
	internal class ImageLoader : IDisposable, IServiceProvider
	{
		private static readonly HttpClient httpClient = new HttpClient();

		private Hashtable imageData;

		private IServiceContainer serviceContainer;

		private ImageLoader()
		{
		}

		public ImageLoader(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(SR.ExceptionImageLoaderInvalidServiceContainer);
			}
			serviceContainer = container;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(ImageLoader))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionImageLoaderUnsupportedType(serviceType.ToString()));
		}

		public void Dispose()
		{
			if (imageData == null)
			{
				return;
			}
			foreach (DictionaryEntry imageDatum in imageData)
			{
				((Image)imageDatum.Value).Dispose();
			}
			imageData = null;
			GC.SuppressFinalize(this);
		}

		public Image LoadImage(string imageURL)
		{
			return LoadImage(imageURL, saveImage: true);
		}

		public Image LoadImage(string imageURL, bool saveImage)
		{
			Image image = null;
			if (serviceContainer != null)
			{
				Chart chart = (Chart)serviceContainer.GetService(typeof(Chart));
				if (chart != null)
				{
					foreach (NamedImage image2 in chart.Images)
					{
						if (image2.Name == imageURL)
						{
							return image2.Image;
						}
					}
				}
			}
			if (imageData == null)
			{
				imageData = new Hashtable(StringComparer.OrdinalIgnoreCase);
			}
			if (imageData.Contains(imageURL))
			{
				image = (Image)imageData[imageURL];
			}
			if (image == null)
			{
				try
				{
					int num = imageURL.IndexOf("::", StringComparison.Ordinal);
					if (num > 0)
					{
						string baseName = imageURL.Substring(0, num);
						string name = imageURL.Substring(num + 2);
						image = (Image)new ResourceManager(baseName, Assembly.GetExecutingAssembly()).GetObject(name);
					}
				}
				catch (Exception)
				{
				}
			}
			if (image == null)
			{
				Uri uri = null;
				try
				{
					uri = new Uri(imageURL);
				}
				catch (Exception)
				{
				}
				if (uri != null)
				{
					try
					{
						using HttpResponseMessage response = httpClient.Send(new HttpRequestMessage(HttpMethod.Get, uri));
						MemoryStream memoryStream = new MemoryStream();
						using (Stream responseStream = response.Content.ReadAsStream())
						{
							responseStream.CopyTo(memoryStream);
						}
						memoryStream.Position = 0;
						image = Image.FromStream(memoryStream);
					}
					catch (Exception)
					{
					}
				}
			}
			if (image == null)
			{
				image = LoadFromFile(imageURL);
			}
			if (image == null)
			{
				throw new ArgumentException(SR.ExceptionImageLoaderIncorrectImageUrl(imageURL));
			}
			if (saveImage)
			{
				imageData[imageURL] = image;
			}
			return image;
		}

		private Image LoadFromFile(string fileName)
		{
			try
			{
				return Image.FromFile(fileName);
			}
			catch (Exception)
			{
				return null;
			}
		}

		internal bool GetAdjustedImageSize(string name, ref SizeF size)
		{
			Image image = LoadImage(name);
			if (image == null)
			{
				return false;
			}
			GetAdjustedImageSize(image, ref size);
			return true;
		}

		/// <summary>
		/// Assumes a fixed 96 DPI baseline (chart-gdi-type-abstraction.md Milestone B2, decision 2026-07-18):
		/// GDI+'s original DPI-mismatch rescaling only mattered for print/high-DPI targets, and required
		/// resolution metadata (Image.HorizontalResolution/VerticalResolution) that has no SkiaSharp equivalent.
		/// No longer takes a Graphics parameter (Milestone D2): it was unused once the 96 DPI baseline
		/// landed, and every caller had to reach through a GDI+-typed ChartGraphics.Graphics just to
		/// supply an argument nothing in this method reads.
		/// </summary>
		internal static void GetAdjustedImageSize(Image image, ref SizeF size)
		{
			size.Width = image.Width;
			size.Height = image.Height;
		}
	}
}
