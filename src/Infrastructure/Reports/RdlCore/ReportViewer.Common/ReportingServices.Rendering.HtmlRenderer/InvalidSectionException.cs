using ReportingServices.HtmlRendering;
using System;
using System.Runtime.Serialization;

namespace ReportingServices.Rendering.HtmlRenderer
{
	[Serializable]
	internal class InvalidSectionException : Exception
	{
		public InvalidSectionException()
			: base(RenderRes.rrInvalidSectionError)
		{
		}
	}
}
