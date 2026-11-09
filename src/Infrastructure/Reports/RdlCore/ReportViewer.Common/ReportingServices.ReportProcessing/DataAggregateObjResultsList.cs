using System.Collections;

namespace ReportingServices.ReportProcessing
{
	internal sealed class DataAggregateObjResultsList : ArrayList
	{
		internal new DataAggregateObjResult[] this[int index] => (DataAggregateObjResult[])base[index];
	}
}
