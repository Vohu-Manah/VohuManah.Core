using ReportingServices.ReportProcessing.Persistence;
using System;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class RecordSetPropertyNames
	{
		private StringList m_propertyNames;

		internal StringList PropertyNames
		{
			get
			{
				return m_propertyNames;
			}
			set
			{
				m_propertyNames = value;
			}
		}

		internal RecordSetPropertyNames()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.PropertyNames, ReportingServices.ReportProcessing.Persistence.ObjectType.StringList));
			return new Declaration(ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
