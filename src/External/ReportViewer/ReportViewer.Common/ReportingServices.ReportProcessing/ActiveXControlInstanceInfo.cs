using ReportingServices.ReportProcessing.Persistence;
using System;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ActiveXControlInstanceInfo : ReportItemInstanceInfo
	{
		private object[] m_parameterValues;

		internal object[] ParameterValues
		{
			get
			{
				return m_parameterValues;
			}
			set
			{
				m_parameterValues = value;
			}
		}

		internal ActiveXControlInstanceInfo(ReportProcessing.ProcessingContext pc, ActiveXControl reportItemDef, ReportItemInstance owner, int index)
			: base(pc, reportItemDef, owner, index)
		{
			if (reportItemDef.Parameters != null)
			{
				m_parameterValues = new object[reportItemDef.Parameters.Count];
			}
		}

		internal ActiveXControlInstanceInfo(ActiveXControl reportItemDef)
			: base(reportItemDef)
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ParameterValues, Token.Array, ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			return new Declaration(ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItemInstanceInfo, memberInfoList);
		}
	}
}
