using ReportingServices.ReportProcessing.Persistence;
using System;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class SortFilterEventInfo
	{
		[Reference]
		private TextBox m_eventSource;

		private VariantList[] m_eventSourceScopeInfo;

		internal TextBox EventSource
		{
			get
			{
				return m_eventSource;
			}
			set
			{
				m_eventSource = value;
			}
		}

		internal VariantList[] EventSourceScopeInfo
		{
			get
			{
				return m_eventSourceScopeInfo;
			}
			set
			{
				m_eventSourceScopeInfo = value;
			}
		}

		internal SortFilterEventInfo()
		{
		}

		internal SortFilterEventInfo(TextBox eventSource)
		{
			m_eventSource = eventSource;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.EventSource, Token.Reference, ReportingServices.ReportProcessing.Persistence.ObjectType.TextBox));
			memberInfoList.Add(new MemberInfo(MemberName.EventSourceScopeInfo, Token.Array, ReportingServices.ReportProcessing.Persistence.ObjectType.VariantList));
			return new Declaration(ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
