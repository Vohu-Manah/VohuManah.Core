using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IStaticReferenceable
	{
		int ID
		{
			get;
		}

		void SetID(int id);

		ObjectType GetObjectType();
	}
}
