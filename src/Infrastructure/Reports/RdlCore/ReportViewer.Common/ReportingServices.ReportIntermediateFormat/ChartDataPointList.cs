using System;

namespace ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ChartDataPointList : CellList
	{
		internal new ChartDataPoint this[int index] => (ChartDataPoint)base[index];

		public ChartDataPointList()
		{
		}

		internal ChartDataPointList(int capacity)
			: base(capacity)
		{
		}
	}
}
