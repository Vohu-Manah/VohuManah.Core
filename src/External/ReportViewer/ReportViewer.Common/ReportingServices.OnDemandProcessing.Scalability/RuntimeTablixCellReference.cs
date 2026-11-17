using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeTablixCellReference : RuntimeCellReference, IReference<RuntimeTablixCell>, IReference, IStorable, IPersistable
	{
		internal RuntimeTablixCellReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeTablixCellReference;
		}

		[DebuggerStepThrough]
		public new RuntimeTablixCell Value()
		{
			return (RuntimeTablixCell)InternalValue();
		}
	}
}
