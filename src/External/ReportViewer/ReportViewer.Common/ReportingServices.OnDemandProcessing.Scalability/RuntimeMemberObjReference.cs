using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeMemberObjReference : Reference<RuntimeMemberObj>
	{
		internal RuntimeMemberObjReference()
		{
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.RuntimeMemberObjReference;
		}
	}
}
