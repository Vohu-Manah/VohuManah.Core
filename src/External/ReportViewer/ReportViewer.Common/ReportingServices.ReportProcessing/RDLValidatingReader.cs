using System;
using System.Collections;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;

namespace ReportingServices.ReportProcessing
{
	// Was `: XmlValidatingReader` (obsolete, CS0618). XmlValidatingReader let a subclass mutate
	// Schemas/ValidationEventHandler/ValidationType *after* construction (RmlValidatingReader in
	// ReportPublishing.cs does exactly this, before any Read() call). XmlReader.Create(reader,
	// settings) has no equivalent - the validating reader is built once, from a fixed settings
	// snapshot. So the inner validating XmlReader is now built lazily on first real use, once a
	// subclass constructor has finished mutating m_settings, instead of eagerly in this
	// constructor - mirrors ReportingServices.ReportPublishing.RDLValidatingReader's own
	// XmlValidatingReader->XmlReader migration (already done), adapted for the constructor shape
	// this class's one real subclass (ReportPublishing.cs's nested RmlValidatingReader) needs.
	internal class RDLValidatingReader : XmlReader
	{
		private sealed class RdlElementStack : ArrayList
		{
			internal new Hashtable this[int index]
			{
				get
				{
					return (Hashtable)base[index];
				}
				set
				{
					base[index] = value;
				}
			}

			internal RdlElementStack()
			{
			}
		}

		private sealed class XmlNullResolver : XmlUrlResolver
		{
			public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
			{
				throw new XmlException("Can't resolve URI reference ", null);
			}
		}

		private RdlElementStack m_rdlElementStack;

		private readonly string m_validationNamespace;

		private readonly XmlReader m_innerReader;

		private readonly XmlReaderSettings m_settings;

		private XmlReader m_reader;

		private XmlReader Reader => m_reader ?? (m_reader = XmlReader.Create(m_innerReader, m_settings));

		public XmlSchemaSet Schemas => m_settings.Schemas;

		public ValidationType ValidationType
		{
			get
			{
				return m_settings.ValidationType;
			}
			set
			{
				m_settings.ValidationType = value;
			}
		}

		public event ValidationEventHandler ValidationEventHandler
		{
			add
			{
				m_settings.ValidationEventHandler += value;
			}
			remove
			{
				m_settings.ValidationEventHandler -= value;
			}
		}

		public XmlResolver XmlResolver
		{
			set
			{
				m_settings.XmlResolver = value;
			}
		}

		internal int LineNumber => (Reader as IXmlLineInfo)?.LineNumber ?? 0;

		internal int LinePosition => (Reader as IXmlLineInfo)?.LinePosition ?? 0;

		public override XmlReaderSettings Settings => Reader.Settings;

		public override int AttributeCount => Reader.AttributeCount;

		public override string BaseURI => Reader.BaseURI;

		public override int Depth => Reader.Depth;

		public override bool EOF => Reader.EOF;

		public override bool HasValue => Reader.HasValue;

		public override bool IsEmptyElement => Reader.IsEmptyElement;

		public override string LocalName => Reader.LocalName;

		public override XmlNameTable NameTable => Reader.NameTable;

		public override string NamespaceURI => Reader.NamespaceURI;

		public override XmlNodeType NodeType => Reader.NodeType;

		public override string Prefix => Reader.Prefix;

		public override ReadState ReadState => Reader.ReadState;

		public override string Value => Reader.Value;

		public RDLValidatingReader(XmlReader xmlReader, string validationNamespace)
		{
			m_innerReader = xmlReader;
			m_validationNamespace = validationNamespace;
			m_settings = new XmlReaderSettings
			{
				XmlResolver = new XmlNullResolver()
			};
		}

		private static int CompareWithInvariantCulture(string x, string y, bool ignoreCase)
		{
			return string.Compare(x, y, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		public bool Validate(out string message)
		{
			message = null;
			if (CompareWithInvariantCulture(m_validationNamespace, Reader.NamespaceURI, ignoreCase: false) != 0)
			{
				return true;
			}
			XmlSchemaComplexType xmlSchemaComplexType = null;
			bool result = true;
			ArrayList arrayList = new ArrayList();
			switch (Reader.NodeType)
			{
			case XmlNodeType.Element:
			{
				if (m_rdlElementStack == null)
				{
					m_rdlElementStack = new RdlElementStack();
				}
				xmlSchemaComplexType = (Reader.SchemaInfo?.SchemaType as XmlSchemaComplexType);
				if (xmlSchemaComplexType != null)
				{
					TraverseParticle(xmlSchemaComplexType.ContentTypeParticle, arrayList);
				}
				if (!Reader.IsEmptyElement)
				{
					if (xmlSchemaComplexType != null && 1 < arrayList.Count && CompareWithInvariantCulture("ReportItemsType", xmlSchemaComplexType.Name, ignoreCase: false) != 0 && CompareWithInvariantCulture("MapLayersType", xmlSchemaComplexType.Name, ignoreCase: false) != 0)
					{
						Hashtable hashtable2 = new Hashtable(arrayList.Count);
						hashtable2.Add("_ParentName", Reader.LocalName);
						hashtable2.Add("_Type", xmlSchemaComplexType);
						m_rdlElementStack.Add(hashtable2);
					}
					else
					{
						m_rdlElementStack.Add(null);
					}
				}
				else if (xmlSchemaComplexType != null)
				{
					for (int j = 0; j < arrayList.Count; j++)
					{
						XmlSchemaElement xmlSchemaElement2 = arrayList[j] as XmlSchemaElement;
						if (xmlSchemaElement2.MinOccurs > 0m)
						{
							result = false;
							message = RDLValidatingReaderStrings.rdlValidationMissingChildElement(Reader.LocalName, xmlSchemaElement2.Name, LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
						}
					}
				}
				if (0 >= Reader.Depth || m_rdlElementStack == null)
				{
					break;
				}
				Hashtable hashtable3 = m_rdlElementStack[Reader.Depth - 1];
				if (hashtable3 != null)
				{
					if (hashtable3.ContainsKey(Reader.LocalName))
					{
						result = false;
						message = RDLValidatingReaderStrings.rdlValidationInvalidElement(hashtable3["_ParentName"] as string, Reader.LocalName, LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
					}
					else
					{
						hashtable3.Add(Reader.LocalName, null);
					}
				}
				break;
			}
			case XmlNodeType.EndElement:
			{
				if (m_rdlElementStack == null)
				{
					break;
				}
				Hashtable hashtable = m_rdlElementStack[m_rdlElementStack.Count - 1];
				if (hashtable != null)
				{
					xmlSchemaComplexType = (hashtable["_Type"] as XmlSchemaComplexType);
					TraverseParticle(xmlSchemaComplexType.ContentTypeParticle, arrayList);
					for (int i = 0; i < arrayList.Count; i++)
					{
						XmlSchemaElement xmlSchemaElement = arrayList[i] as XmlSchemaElement;
						if (xmlSchemaElement.MinOccurs > 0m && !hashtable.ContainsKey(xmlSchemaElement.Name))
						{
							result = false;
							message = RDLValidatingReaderStrings.rdlValidationMissingChildElement(Reader.LocalName, xmlSchemaElement.Name, LineNumber.ToString(CultureInfo.InvariantCulture.NumberFormat), LinePosition.ToString(CultureInfo.InvariantCulture.NumberFormat));
						}
					}
					m_rdlElementStack[m_rdlElementStack.Count - 1] = null;
				}
				m_rdlElementStack.RemoveAt(m_rdlElementStack.Count - 1);
				break;
			}
			}
			return result;
		}

		private static void TraverseParticle(XmlSchemaParticle particle, ArrayList elementDeclsInContentModel)
		{
			if (particle is XmlSchemaElement)
			{
				XmlSchemaElement value = particle as XmlSchemaElement;
				elementDeclsInContentModel.Add(value);
			}
			else
			{
				if (!(particle is XmlSchemaGroupBase))
				{
					return;
				}
				foreach (XmlSchemaParticle item in (particle as XmlSchemaGroupBase).Items)
				{
					TraverseParticle(item, elementDeclsInContentModel);
				}
			}
		}

		public override void Close()
		{
			Reader.Close();
		}

		public override string GetAttribute(int i)
		{
			return Reader.GetAttribute(i);
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			return Reader.GetAttribute(name, namespaceURI);
		}

		public override string GetAttribute(string name)
		{
			return Reader.GetAttribute(name);
		}

		public override string LookupNamespace(string prefix)
		{
			return Reader.LookupNamespace(prefix);
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			return Reader.MoveToAttribute(name, ns);
		}

		public override bool MoveToAttribute(string name)
		{
			return Reader.MoveToAttribute(name);
		}

		public override bool MoveToElement()
		{
			return Reader.MoveToElement();
		}

		public override bool MoveToFirstAttribute()
		{
			return Reader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			return Reader.MoveToNextAttribute();
		}

		public override bool Read()
		{
			return Reader.Read();
		}

		public override bool ReadAttributeValue()
		{
			return Reader.ReadAttributeValue();
		}

		public override void ResolveEntity()
		{
			Reader.ResolveEntity();
		}
	}
}
