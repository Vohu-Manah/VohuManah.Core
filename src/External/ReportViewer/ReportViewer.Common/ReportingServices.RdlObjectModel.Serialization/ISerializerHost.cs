using System;
using System.Collections.Generic;

namespace ReportingServices.RdlObjectModel.Serialization
{
	internal interface ISerializerHost
	{
		Type GetSubstituteType(Type type);

		void OnDeserialization(object value);

		IEnumerable<ExtensionNamespace> GetExtensionNamespaces();
	}
}
