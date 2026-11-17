using ReportingServices.ReportProcessing.Persistence;
using System;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class PageSectionInstanceInfo : ReportItemInstanceInfo
	{
		internal PageSectionInstanceInfo(ReportProcessing.ProcessingContext pc, PageSection reportItemDef, PageSectionInstance owner)
			: base(pc, reportItemDef, owner, addToChunk: true)
		{
		}

		internal PageSectionInstanceInfo(PageSection reportItemDef)
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
