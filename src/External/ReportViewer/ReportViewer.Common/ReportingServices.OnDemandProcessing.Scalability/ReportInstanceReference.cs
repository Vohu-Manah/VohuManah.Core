using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class ReportInstanceReference : ScopeInstanceReference, IReference<ReportInstance>, IReference, IStorable, IPersistable
	{
		internal ReportInstanceReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.ReportInstanceReference;
		}

		public new ReportInstance Value()
		{
			return (ReportInstance)InternalValue();
		}
	}
}
