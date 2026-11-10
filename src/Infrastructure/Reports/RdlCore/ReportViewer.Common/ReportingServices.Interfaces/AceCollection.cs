using System;
using System.Collections;

namespace ReportingServices.Interfaces
{
	[Serializable]
	public sealed class AceCollection : CollectionBase
	{
		public AceStruct this[int index] => (AceStruct)base.InnerList[index];

		public int Add(AceStruct ace)
		{
			return base.InnerList.Add(ace);
		}
	}
}
