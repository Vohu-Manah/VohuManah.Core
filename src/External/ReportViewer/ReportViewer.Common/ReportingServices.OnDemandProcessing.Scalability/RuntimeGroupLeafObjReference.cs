using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;
using System.Diagnostics;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeGroupLeafObjReference : RuntimeGroupObjReference, IReference<RuntimeGroupLeafObj>, IReference, IStorable, IPersistable
	{
		internal RuntimeGroupLeafObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeGroupLeafObjReference;
		}

		[DebuggerStepThrough]
		public new RuntimeGroupLeafObj Value()
		{
			return (RuntimeGroupLeafObj)InternalValue();
		}
	}
}
