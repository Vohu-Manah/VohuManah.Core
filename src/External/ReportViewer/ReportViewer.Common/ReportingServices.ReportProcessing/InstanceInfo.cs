using ReportingServices.ReportProcessing.Persistence;
using System;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal abstract class InstanceInfo : InfoBase
	{
		internal new static Declaration GetDeclaration()
		{
			MemberInfoList members = new MemberInfoList();
			return new Declaration(ReportingServices.ReportProcessing.Persistence.ObjectType.InfoBase, members);
		}
	}
}
