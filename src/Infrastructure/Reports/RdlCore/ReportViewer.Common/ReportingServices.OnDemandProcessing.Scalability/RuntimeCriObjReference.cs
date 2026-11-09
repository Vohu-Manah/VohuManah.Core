using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeCriObjReference : RuntimeChartCriObjReference, IReference<RuntimeCriObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeCriObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeCriObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeCriObj Value()
		{
			return (RuntimeCriObj)InternalValue();
		}
	}
}
