using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class CommonReferenceCreator : IReferenceCreator
	{
		private static CommonReferenceCreator m_instance;

		internal static CommonReferenceCreator Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new CommonReferenceCreator();
				}
				return m_instance;
			}
		}

		private CommonReferenceCreator()
		{
		}

		public bool TryCreateReference(IStorable refTarget, out BaseReference newReference)
		{
			switch (refTarget.GetObjectType())
			{
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArray:
				return TryCreateReference(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArrayReference, out newReference);
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNode:
				return TryCreateReference(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference, out newReference);
			default:
				newReference = null;
				return false;
			}
		}

		public bool TryCreateReference(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceObjectType, out BaseReference reference)
		{
			switch (referenceObjectType)
			{
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Null:
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None:
				Global.Tracer.Assert(condition: false, "Cannot create reference to Nothing or Null");
				reference = null;
				return false;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArrayReference:
				reference = new SimpleReference<StorableArray>(referenceObjectType);
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference:
				reference = new ScalableDictionaryNodeReference();
				break;
			default:
				reference = null;
				return false;
			}
			return true;
		}
	}
}
