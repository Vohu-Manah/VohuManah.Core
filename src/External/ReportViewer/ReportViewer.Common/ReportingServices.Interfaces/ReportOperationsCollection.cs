using System;
using System.Collections;

namespace ReportingServices.Interfaces
{
	[Serializable]
	public sealed class ReportOperationsCollection : CollectionBase
	{
		public ReportOperation this[int index] => (ReportOperation)base.InnerList[index];

		public int Add(ReportOperation operation)
		{
			return base.InnerList.Add(operation);
		}
	}
}
