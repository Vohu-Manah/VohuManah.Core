using ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	[ArrayOfReferences]
	internal sealed class DataRegionList : ArrayList
	{
		internal new DataRegion this[int index] => (DataRegion)base[index];

		internal DataRegionList()
		{
		}

		internal DataRegionList(int capacity)
			: base(capacity)
		{
		}
	}
}
