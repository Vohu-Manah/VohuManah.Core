using ReportingServices.ReportProcessing.ExprHostObjectModel;
using ReportingServices.ReportProcessing.Persistence;
using ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class PlotArea
	{
		internal enum Origins
		{
			BottomLeft,
			TopLeft,
			TopRight,
			BottomRight
		}

		private Origins m_origin;

		private Style m_styleClass;

		internal Origins Origin
		{
			get
			{
				return m_origin;
			}
			set
			{
				m_origin = value;
			}
		}

		internal Style StyleClass
		{
			get
			{
				return m_styleClass;
			}
			set
			{
				m_styleClass = value;
			}
		}

		internal void SetExprHost(StyleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			exprHost.SetReportObjectModel(reportObjectModel);
			if (m_styleClass != null)
			{
				m_styleClass.SetStyleExprHost(exprHost);
			}
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartPlotAreaStart();
			if (m_styleClass != null)
			{
				m_styleClass.Initialize(context);
			}
			context.ExprHostBuilder.ChartPlotAreaEnd();
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Origin, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.StyleClass, ReportingServices.ReportProcessing.Persistence.ObjectType.Style));
			return new Declaration(ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
