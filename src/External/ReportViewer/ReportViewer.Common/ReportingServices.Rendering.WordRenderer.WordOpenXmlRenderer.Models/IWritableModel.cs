using System.Collections.Generic;
using System.IO;

namespace ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal interface IWritableModel
	{
		void Write(TextWriter s, Dictionary<string, string> namespaces);
	}
}
