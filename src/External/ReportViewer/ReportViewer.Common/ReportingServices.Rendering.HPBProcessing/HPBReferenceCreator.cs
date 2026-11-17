using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat.Persistence;

namespace ReportingServices.Rendering.HPBProcessing
{
	internal class HPBReferenceCreator : IReferenceCreator
	{
		private static HPBReferenceCreator m_instance = new HPBReferenceCreator();

		internal static HPBReferenceCreator Instance => m_instance;

		private HPBReferenceCreator()
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
