using ReportingServices.Rendering.RPLProcessing;
using System.IO;

namespace ReportingServices.Rendering.HtmlRenderer
{
	internal interface UrlWriter
	{
		void WriteImage(Stream destinationStream, RPLImageData image);
	}
}
