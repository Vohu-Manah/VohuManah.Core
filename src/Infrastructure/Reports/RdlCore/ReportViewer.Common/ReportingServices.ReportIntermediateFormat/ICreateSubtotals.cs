using ReportingServices.ReportPublishing;

namespace ReportingServices.ReportIntermediateFormat
{
	internal interface ICreateSubtotals
	{
		void CreateAutomaticSubtotals(AutomaticSubtotalContext context);
	}
}
