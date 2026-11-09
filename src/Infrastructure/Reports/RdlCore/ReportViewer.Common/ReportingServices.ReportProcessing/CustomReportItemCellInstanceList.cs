using System;
using System.Collections;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CustomReportItemCellInstanceList : ArrayList
	{
		internal new CustomReportItemCellInstance this[int index] => (CustomReportItemCellInstance)base[index];

		internal CustomReportItemCellInstanceList()
		{
		}

		internal CustomReportItemCellInstanceList(int capacity)
			: base(capacity)
		{
		}
	}
}
