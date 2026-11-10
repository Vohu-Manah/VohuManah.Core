using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.Rendering.SPBProcessing
{
	internal class SPBReferenceCreator : IReferenceCreator
	{
		private static SPBReferenceCreator m_instance = new SPBReferenceCreator();

		internal static SPBReferenceCreator Instance => m_instance;

		private SPBReferenceCreator()
		{
		}

		public bool TryCreateReference(IStorable refTarget, out BaseReference reference)
		{
			reference = null;
			return false;
		}

		public bool TryCreateReference(ObjectType referenceObjectType, out BaseReference reference)
		{
			reference = null;
			return false;
		}
	}
}
