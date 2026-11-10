using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class LookupReferenceCreator : IReferenceCreator
	{
		private static LookupReferenceCreator m_instance;

		internal static LookupReferenceCreator Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new LookupReferenceCreator();
				}
				return m_instance;
			}
		}

		private LookupReferenceCreator()
		{
		}

		public bool TryCreateReference(IStorable refTarget, out BaseReference newReference)
		{
			ReportingServices.ReportIntermediateFormat.Persistence.ObjectType objectType = refTarget.GetObjectType();
			if (TryMapObjectTypeToReferenceType(objectType, out ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceType))
			{
				return TryCreateReference(referenceType, out newReference);
			}
			newReference = null;
			return false;
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
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTableReference:
				reference = new SimpleReference<LookupTable>(referenceObjectType);
				return true;
			default:
				reference = null;
				return false;
			}
		}

		private bool TryMapObjectTypeToReferenceType(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType targetType, out ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceType)
		{
			if (targetType == ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTable)
			{
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTableReference;
				return true;
			}
			referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None;
			return false;
		}
	}
}
