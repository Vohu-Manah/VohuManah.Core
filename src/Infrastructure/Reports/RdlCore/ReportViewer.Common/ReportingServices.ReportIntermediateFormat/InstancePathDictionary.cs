using System.Collections.Generic;

namespace ReportingServices.ReportIntermediateFormat
{
	internal sealed class InstancePathDictionary<TValue> : Dictionary<List<InstancePathItem>, TValue>
	{
		public InstancePathDictionary()
			: base((IEqualityComparer<List<InstancePathItem>>)InstancePathComparer.Instance)
		{
		}

		public InstancePathDictionary(int capacity)
			: base(capacity, (IEqualityComparer<List<InstancePathItem>>)InstancePathComparer.Instance)
		{
		}
	}
}
