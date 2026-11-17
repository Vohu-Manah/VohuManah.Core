using ReportingServices.RdlObjectModel;

using ReportingServices.RdlObjectModel2005.Upgrade;
using Action = ReportingServices.RdlObjectModel.Action;



namespace ReportingServices.RdlObjectModel2005

{

	internal interface IReportItem2005 : IUpgradeable

	{

		Action Action

		{

			get;

			set;

		}

	}

}
