using ReportingServices.ReportProcessing.Persistence;
using System;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CustomReportItemInstanceInfo : ReportItemInstanceInfo
	{
		internal CustomReportItemInstanceInfo(ReportProcessing.ProcessingContext pc, CustomReportItem reportItemDef, CustomReportItemInstance owner)
			: base(pc, reportItemDef, owner, addToChunk: true)
		{
		}

		internal CustomReportItemInstanceInfo(CustomReportItem reportItemDef)
			: base(reportItemDef)
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList members = new MemberInfoList();
			return new Declaration(ReportingServices.ReportProcessing.Persistence.ObjectType.None, members);
		}
	}
}
