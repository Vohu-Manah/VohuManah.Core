using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeGroupRootObjReference : RuntimeGroupObjReference, IReference<RuntimeGroupRootObj>, IReference, IStorable, IPersistable, IReference<IDataCorrelation>
	{
		internal RuntimeGroupRootObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeGroupRootObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeGroupRootObj Value()
		{
			return (RuntimeGroupRootObj)InternalValue();
		}

		[DebuggerStepThrough]
		IDataCorrelation IReference<IDataCorrelation>.Value()
		{
			return (IDataCorrelation)InternalValue();
		}
	}
}
