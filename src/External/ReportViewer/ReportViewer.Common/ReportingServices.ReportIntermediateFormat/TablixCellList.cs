using System;

namespace ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class TablixCellList : CellList
	{
		internal new TablixCell this[int index] => (TablixCell)base[index];

		public TablixCellList()
		{
		}

		internal TablixCellList(int capacity)
			: base(capacity)
		{
		}
	}
}
