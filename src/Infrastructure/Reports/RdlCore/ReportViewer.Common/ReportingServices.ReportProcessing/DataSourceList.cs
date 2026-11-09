using System;
using System.Collections;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataSourceList : ArrayList
	{
		internal new DataSource this[int index] => (DataSource)base[index];

		internal DataSourceList()
		{
		}

		internal DataSourceList(int capacity)
			: base(capacity)
		{
		}
	}
}
