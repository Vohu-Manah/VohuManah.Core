using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class ScopeInstanceReference : Reference<ScopeInstance>
	{
		internal ScopeInstanceReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.ScopeInstanceReference;
		}

		public ScopeInstance Value()
		{
			return (ScopeInstance)InternalValue();
		}
	}
}
