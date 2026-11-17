using System;

namespace ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ChartMemberList : HierarchyNodeList
	{
		internal new ChartMember this[int index] => (ChartMember)base[index];

		public ChartMemberList()
		{
		}

		internal ChartMemberList(int capacity)
			: base(capacity)
		{
		}
	}
}
