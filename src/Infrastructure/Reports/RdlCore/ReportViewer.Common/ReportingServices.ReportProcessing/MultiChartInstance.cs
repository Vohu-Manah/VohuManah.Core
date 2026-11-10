using ReportingServices.ReportProcessing.Persistence;
using System;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MultiChartInstance
	{
		private ChartHeadingInstanceList m_columnInstances;

		private ChartHeadingInstanceList m_rowInstances;

		private ChartDataPointInstancesList m_cellDataPoints;

		[NonSerialized]
		private ChartHeadingInstanceList m_innerHeadingInstanceList;

		internal ChartHeadingInstanceList ColumnInstances
		{
			get
			{
				return m_columnInstances;
			}
			set
			{
				m_columnInstances = value;
			}
		}

		internal ChartHeadingInstanceList RowInstances
		{
			get
			{
				return m_rowInstances;
			}
			set
			{
				m_rowInstances = value;
			}
		}

		internal ChartDataPointInstancesList DataPoints
		{
			get
			{
				return m_cellDataPoints;
			}
			set
			{
				m_cellDataPoints = value;
			}
		}

		internal ChartHeadingInstanceList InnerHeadingInstanceList
		{
			get
			{
				return m_innerHeadingInstanceList;
			}
			set
			{
				m_innerHeadingInstanceList = value;
			}
		}

		internal MultiChartInstance(Chart reportItemDef)
		{
			m_columnInstances = new ChartHeadingInstanceList();
			m_rowInstances = new ChartHeadingInstanceList();
			m_cellDataPoints = new ChartDataPointInstancesList();
		}

		internal MultiChartInstance()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ColumnInstances, ReportingServices.ReportProcessing.Persistence.ObjectType.ChartHeadingInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.RowInstances, ReportingServices.ReportProcessing.Persistence.ObjectType.ChartHeadingInstanceList));
			memberInfoList.Add(new MemberInfo(MemberName.CellDataPoints, ReportingServices.ReportProcessing.Persistence.ObjectType.ChartDataPointInstancesList));
			return new Declaration(ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
