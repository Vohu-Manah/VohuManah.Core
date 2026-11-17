using System.Drawing;

namespace ReportingServices.Rendering.ImageRenderer
{
	internal sealed class PDFUriAction
	{
		internal string Uri;

		internal RectangleF Rectangle;

		internal PDFUriAction(string uri, RectangleF rectangle)
		{
			Uri = uri;
			Rectangle = rectangle;
		}
	}
}
