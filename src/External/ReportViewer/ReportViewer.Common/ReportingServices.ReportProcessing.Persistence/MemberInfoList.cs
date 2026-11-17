using System.Collections;

namespace ReportingServices.ReportProcessing.Persistence
{
	internal sealed class MemberInfoList : ArrayList
	{
		internal new MemberInfo this[int index] => (MemberInfo)base[index];

		internal MemberInfoList()
		{
		}

		internal MemberInfoList(int capacity)
			: base(capacity)
		{
		}
	}
}
