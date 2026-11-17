using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Reporting.Map.WebForms
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
				throw new ArgumentNullException("ImageLoader - Valid Service Container object must be provided");
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
			throw new ArgumentException("ImageLoader - Image loader do not provide service of type: " + serviceType.ToString());
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
				MapCore mapCore = (MapCore)serviceContainer.GetService(typeof(MapCore));
				if (mapCore != null)
				{
					foreach (NamedImage namedImage in mapCore.NamedImages)
					{
						if (namedImage.Name == imageURL)
						{
							return namedImage.Image;
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
				Uri uri = null;
				try
				{
					uri = new Uri(imageURL);
				}
				catch (Exception)
				{
				}
				if (uri == null)
				{
					MapControl mapControl = (MapControl)serviceContainer.GetService(typeof(MapControl));
					if (mapControl != null)
					{
						if (imageURL.StartsWith("~", StringComparison.Ordinal) && !string.IsNullOrEmpty(mapControl.applicationDocumentURL))
						{
							try
							{
								uri = new Uri(mapControl.applicationDocumentURL + imageURL.Substring(1));
							}
							catch (Exception)
							{
							}
						}
						else
						{
							string text = mapControl.webFormDocumentURL;
							int num = text.LastIndexOf('/');
							if (num != -1)
							{
								text = text.Substring(0, num + 1);
							}
							try
							{
								uri = new Uri(new Uri(text), imageURL);
							}
							catch (Exception)
							{
							}
						}
					}
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
				throw new ArgumentException(SR.ExceptionCannotLoadImageFromLocation(imageURL));
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
	}
}
