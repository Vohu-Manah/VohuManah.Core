using ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DrillthroughParameters : NameObjectCollectionBase, INameObjectCollection
	{
		public DrillthroughParameters()
		{
		}

		internal DrillthroughParameters(int capacity)
			: base(capacity)
		{
		}

		public void Add(string key, object value)
		{
			BaseAdd(key, value);
		}

		public string GetKey(int index)
		{
			return BaseGetKey(index);
		}

		public object GetValue(int index)
		{
			return BaseGet(index);
		}
	}
}
