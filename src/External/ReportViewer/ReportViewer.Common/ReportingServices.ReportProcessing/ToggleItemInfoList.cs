using System.Collections;

namespace ReportingServices.ReportProcessing
{
	internal sealed class ToggleItemInfoList : ArrayList
	{
		internal new ToggleItemInfo this[int index] => (ToggleItemInfo)base[index];
	}
}
