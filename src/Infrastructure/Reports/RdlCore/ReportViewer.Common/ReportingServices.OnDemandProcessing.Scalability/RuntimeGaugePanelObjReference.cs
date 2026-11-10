using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeGaugePanelObjReference : RuntimeChartCriObjReference, IReference<RuntimeGaugePanelObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeGaugePanelObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeGaugePanelObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeGaugePanelObj Value()
		{
			return (RuntimeGaugePanelObj)InternalValue();
		}
	}
}
