using ReportingServices.OnDemandProcessing.TablixProcessing;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;

namespace ReportingServices.OnDemandProcessing.Scalability
{
	internal class RuntimeReferenceCreator : IReferenceCreator
	{
		private static RuntimeReferenceCreator m_instance = new RuntimeReferenceCreator();

		internal static RuntimeReferenceCreator Instance => m_instance;

		private RuntimeReferenceCreator()
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
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixCellReference:
				reference = new RuntimeTablixCellReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellReference:
				reference = new RuntimeCellReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDetailObjReference:
				reference = new RuntimeDetailObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixObjReference:
				reference = new RuntimeDataTablixObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObjReference:
				reference = new RuntimeHierarchyObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObjReference:
				reference = new RuntimeDataRegionObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupRootObjReference:
				reference = new RuntimeDataTablixGroupRootObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupRootObjReference:
				reference = new RuntimeGroupRootObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObjReference:
				reference = new RuntimeGroupObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixGroupLeafObjReference:
				reference = new RuntimeTablixGroupLeafObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriGroupLeafObjReference:
				reference = new RuntimeChartCriGroupLeafObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObjReference:
				reference = new RuntimeDataTablixGroupLeafObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObjReference:
				reference = new RuntimeGroupLeafObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeOnDemandDataSetObjReference:
				reference = new RuntimeOnDemandDataSetObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortHierarchyObjReference:
				reference = new RuntimeSortHierarchyObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixObjReference:
				reference = new RuntimeTablixObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartObjReference:
				reference = new RuntimeChartObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGaugePanelObjReference:
				reference = new RuntimeGaugePanelObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMapDataRegionObjReference:
				reference = new RuntimeMapDataRegionObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCriObjReference:
				reference = new RuntimeCriObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AggregateRowReference:
				reference = new SimpleReference<AggregateRow>(referenceObjectType);
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateReference:
				reference = new SimpleReference<ReportingServices.ReportIntermediateFormat.DataAggregate>(referenceObjectType);
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjReference:
				reference = new SimpleReference<ReportingServices.ReportIntermediateFormat.DataAggregateObj>(referenceObjectType);
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRowReference:
				reference = new SimpleReference<DataFieldRow>(referenceObjectType);
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IHierarchyObjReference:
				reference = new SimpleReference<IHierarchyObj>(referenceObjectType);
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellsReference:
				reference = new SimpleReference<RuntimeCells>(referenceObjectType);
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriCellReference:
				reference = new RuntimeChartCriCellReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriObjReference:
				reference = new RuntimeChartCriObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixMemberObjReference:
				reference = new RuntimeDataTablixMemberObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjReference:
				reference = new SimpleReference<RuntimeGroupingObj>(referenceObjectType);
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObjReference:
				reference = new RuntimeMemberObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRDLDataRegionObjReference:
				reference = new SimpleReference<RuntimeRDLDataRegionObj>(referenceObjectType);
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRICollectionReference:
				reference = new SimpleReference<RuntimeRICollection>(referenceObjectType);
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortFilterEventInfoReference:
				reference = new SimpleReference<ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeSortFilterEventInfo>(referenceObjectType);
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeUserSortTargetInfoReference:
				reference = new SimpleReference<ReportingServices.OnDemandProcessing.TablixProcessing.RuntimeUserSortTargetInfo>(referenceObjectType);
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortExpressionScopeInstanceHolderReference:
				reference = new SortExpressionScopeInstanceHolderReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterExpressionScopeObjReference:
				reference = new SortFilterExpressionScopeObjReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArrayReference:
				reference = new SimpleReference<StorableArray>(referenceObjectType);
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference:
				reference = new ScalableDictionaryNodeReference();
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTableReference:
				reference = new SimpleReference<LookupTable>(referenceObjectType);
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
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataRegionObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRowReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ObjectModelImpl:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeOnDemandDataSetObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortHierarchyObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortHierarchyObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IHierarchyObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IHierarchyObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortFilterEventInfo:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeSortFilterEventInfoReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterExpressionScopeObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortFilterExpressionScopeObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortExpressionScopeInstanceHolder:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SortExpressionScopeInstanceHolderReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCell:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataAggregateObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRICollection:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRICollectionReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixCell:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixCellReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriCell:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriCellReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRDLDataRegionObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRDLDataRegionObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeUserSortTargetInfo:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeUserSortTargetInfoReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AggregateRow:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AggregateRowReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScope:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IScopeReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCells:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCellsReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupLeafObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeHierarchyObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupingObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupLeafObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDetailObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDetailObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupRootObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGroupRootObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMemberObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupRootObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixGroupRootObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixMemberObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixMemberObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGaugePanelObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeGaugePanelObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMapDataRegionObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeMapDataRegionObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCriObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeCriObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixGroupLeafObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeTablixGroupLeafObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriGroupLeafObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeChartCriGroupLeafObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeOnDemandDataSetObj:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeOnDemandDataSetObjReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArray:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StorableArrayReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNode:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalableDictionaryNodeReference;
				break;
			case ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTable:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.LookupTableReference;
				break;
			default:
				referenceType = ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None;
				return false;
			}
			return true;
		}
	}
}
