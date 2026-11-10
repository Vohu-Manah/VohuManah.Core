using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class DataRegionInstanceReference : ScopeInstanceReference, IReference<DataRegionInstance>, IReference, IStorable, IPersistable
	{
		internal DataRegionInstanceReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.DataRegionInstanceReference;
		}

		public new DataRegionInstance Value()
		{
			return (DataRegionInstance)InternalValue();
		}
	}
}
