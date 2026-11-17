using System;
using System.Collections;

namespace ReportingServices.Interfaces
{
	[Serializable]
	public sealed class DatasourceOperationsCollection : CollectionBase
	{
		public DatasourceOperation this[int index] => (DatasourceOperation)base.InnerList[index];

		public int Add(DatasourceOperation operation)
		{
			return base.InnerList.Add(operation);
		}
	}
}
