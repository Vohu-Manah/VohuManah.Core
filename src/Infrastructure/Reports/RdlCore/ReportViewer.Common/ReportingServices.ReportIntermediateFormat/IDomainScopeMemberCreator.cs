using ReportingServices.ReportPublishing;

namespace ReportingServices.ReportIntermediateFormat
{
	internal interface IDomainScopeMemberCreator
	{
		void CreateDomainScopeMember(ReportHierarchyNode parentNode, Grouping grouping, AutomaticSubtotalContext context);
	}
}
