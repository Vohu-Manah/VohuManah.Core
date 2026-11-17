using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class GroupTreeReferenceCreator : IReferenceCreator
	{
		private static GroupTreeReferenceCreator m_instance;

		internal static GroupTreeReferenceCreator Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new GroupTreeReferenceCreator();
				}
				return m_instance;
			}
		}

		private GroupTreeReferenceCreator()
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
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstanceReference:
				reference = new DataRegionInstanceReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstanceReference:
				reference = new SubReportInstanceReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstanceReference:
				reference = new ReportInstanceReference();
				break;
			default:
				reference = null;
				return false;
			}
			return true;
		}

		private bool TryMapObjectTypeToReferenceType(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType targetType, out ReportingServices.ReportIntermediateFormat.Persistence.ObjectType referenceType)
		{
			switch (targetType)
			{
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstance:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataRegionInstanceReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstance:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReportInstanceReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstance:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportInstanceReference;
				break;
			default:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None;
				return false;
			}
			return true;
		}
	}
}
