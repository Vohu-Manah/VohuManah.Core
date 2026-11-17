using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeMapDataRegionObjReference : RuntimeChartCriObjReference, IReference<RuntimeMapDataRegionObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeMapDataRegionObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeMapDataRegionObjReference;
		}

		public new RuntimeMapDataRegionObj Value()
		{
			return (RuntimeMapDataRegionObj)InternalValue();
		}
	}
}
