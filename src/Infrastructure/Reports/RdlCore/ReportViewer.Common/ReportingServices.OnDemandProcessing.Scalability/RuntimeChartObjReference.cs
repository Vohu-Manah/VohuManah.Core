using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeChartObjReference : RuntimeChartCriObjReference, IReference<RuntimeChartObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeChartObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeChartObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeChartObj Value()
		{
			return (RuntimeChartObj)InternalValue();
		}
	}
}
