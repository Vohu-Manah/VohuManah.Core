using ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using ReportingServices.ReportProcessing.ReportObjectModel;
using System.Security.Permissions;

namespace ReportingServices.RdlExpressions.ExpressionHostObjectModel
{
	public interface IReportObjectModelProxyForCustomCode
	{
		Parameters Parameters
		{
			get;
		}

		Globals Globals
		{
			get;
		}

		User User
		{
			get;
		}

		Variables Variables
		{
			get;
		}
	}
}
