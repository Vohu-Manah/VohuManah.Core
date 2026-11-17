using System.Collections;

namespace ReportingServices.ReportProcessing
{
	internal sealed class DataAggregateObjList : ArrayList
	{
		internal new DataAggregateObj this[int index] => (DataAggregateObj)base[index];
	}
}
