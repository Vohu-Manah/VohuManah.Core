using System;
using System.Collections;

namespace ReportingServices.Interfaces
{
	[Serializable]
	public sealed class CatalogOperationsCollection : CollectionBase
	{
		public CatalogOperation this[int index] => (CatalogOperation)base.InnerList[index];

		public int Add(CatalogOperation operation)
		{
			return base.InnerList.Add(operation);
		}
	}
}
