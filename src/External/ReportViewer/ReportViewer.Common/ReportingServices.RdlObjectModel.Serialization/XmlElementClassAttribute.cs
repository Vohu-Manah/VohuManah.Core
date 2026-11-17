using System;
using System.Xml.Serialization;

namespace ReportingServices.RdlObjectModel.Serialization
{
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
	internal sealed class XmlElementClassAttribute : XmlElementAttribute
	{
		public XmlElementClassAttribute(string elementName)
			: base(elementName)
		{
		}

		public XmlElementClassAttribute(string elementName, Type type)
			: base(elementName, type)
		{
		}
	}
}
