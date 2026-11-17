using ReportingServices.ReportProcessing.Persistence;
using System;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableDetailInstanceInfo : InstanceInfo
	{
		private bool m_startHidden;

		internal bool StartHidden
		{
			get
			{
				return m_startHidden;
			}
			set
			{
				m_startHidden = value;
			}
		}

		internal TableDetailInstanceInfo(ReportProcessing.ProcessingContext pc, TableDetail tableDetailDef, TableDetailInstance owner, Table tableDef)
		{
			if (pc.ShowHideType != 0)
			{
				m_startHidden = pc.ProcessReceiver(owner.UniqueName, tableDetailDef.Visibility, tableDetailDef.ExprHost, tableDef.ObjectType, tableDef.Name);
			}
			tableDetailDef.StartHidden = m_startHidden;
			pc.ChunkManager.AddInstance(this, owner, pc.InPageSection);
		}

		internal TableDetailInstanceInfo()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.StartHidden, Token.Boolean));
			return new Declaration(ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfo, memberInfoList);
		}
	}
}
