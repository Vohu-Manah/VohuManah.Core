using System.Collections;

namespace ReportingServices.ReportProcessing
{
	internal sealed class RuntimeSortFilterEventInfoList : ArrayList
	{
		internal new RuntimeSortFilterEventInfo this[int index] => (RuntimeSortFilterEventInfo)base[index];

		internal RuntimeSortFilterEventInfoList()
		{
		}
	}
}
