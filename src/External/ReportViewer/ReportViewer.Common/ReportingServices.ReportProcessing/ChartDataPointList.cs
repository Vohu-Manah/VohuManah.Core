using System;
using System.Collections;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartDataPointList : ArrayList
	{
		internal new ChartDataPoint this[int index] => (ChartDataPoint)base[index];

		internal ChartDataPointList()
		{
		}

		internal ChartDataPointList(int capacity)
			: base(capacity)
		{
		}
	}
}
