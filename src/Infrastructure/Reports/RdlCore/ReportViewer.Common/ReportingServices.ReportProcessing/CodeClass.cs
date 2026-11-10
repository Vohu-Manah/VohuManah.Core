using ReportingServices.ReportProcessing.Persistence;
using System;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal struct CodeClass
	{
		internal string ClassName;

		internal string InstanceName;

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ClassName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.InstanceName, Token.String));
			return new Declaration(ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
