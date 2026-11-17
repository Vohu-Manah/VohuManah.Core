using System.Collections;

namespace ReportingServices.ReportProcessing
{
	internal sealed class FiltersList : ArrayList
	{
		internal new Filters this[int index] => (Filters)base[index];
	}
}
