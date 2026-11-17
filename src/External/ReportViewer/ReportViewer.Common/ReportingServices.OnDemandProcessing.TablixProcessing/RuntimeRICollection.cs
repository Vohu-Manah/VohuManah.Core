using ReportingServices.OnDemandProcessing.Scalability;
using ReportingServices.ReportIntermediateFormat;
using ReportingServices.ReportIntermediateFormat.Persistence;
using ReportingServices.ReportProcessing;
using ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;

namespace ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class RuntimeRICollection : IStorable, IPersistable
	{
		private List<RuntimeDataTablixObjReference> m_dataRegionObjs;

		private static Declaration m_declaration = GetDeclaration();

		public int Size => ItemSizes.SizeOf(m_dataRegionObjs);

		internal RuntimeRICollection()
		{
		}

		internal RuntimeRICollection(int capacity)
		{
			m_dataRegionObjs = new List<RuntimeDataTablixObjReference>(capacity);
		}

		internal RuntimeRICollection(IReference<IScope> owner, List<ReportingServices.ReportIntermediateFormat.ReportItem> reportItems, ref DataActions dataAction, OnDemandProcessingContext odpContext)
		{
			m_dataRegionObjs = new List<RuntimeDataTablixObjReference>();
			AddItems(owner, reportItems, ref dataAction, odpContext);
		}

		internal RuntimeRICollection(IReference<IScope> outerScope, List<ReportingServices.ReportIntermediateFormat.DataRegion> dataRegionDefs, OnDemandProcessingContext odpContext, bool onePass)
		{
			m_dataRegionObjs = new List<RuntimeDataTablixObjReference>(dataRegionDefs.Count);
			DataActions dataAction = DataActions.None;
			for (int i = 0; i < dataRegionDefs.Count; i++)
			{
				CreateDataRegions(outerScope, dataRegionDefs[i], odpContext, onePass, ref dataAction);
			}
		}

		public void AddItems(IReference<IScope> owner, List<ReportingServices.ReportIntermediateFormat.ReportItem> reportItems, ref DataActions dataAction, OnDemandProcessingContext odpContext)
		{
			if (reportItems != null && reportItems.Count > 0)
			{
				CreateDataRegions(owner, reportItems, odpContext, onePass: false, ref dataAction);
			}
		}

		private void CreateDataRegions(IReference<IScope> owner, List<ReportingServices.ReportIntermediateFormat.ReportItem> computedRIs, OnDemandProcessingContext odpContext, bool onePass, ref DataActions dataAction)
		{
			if (computedRIs != null)
			{
				for (int i = 0; i < computedRIs.Count; i++)
				{
					ReportingServices.ReportIntermediateFormat.ReportItem reportItem = computedRIs[i];
					CreateDataRegions(owner, reportItem, odpContext, onePass, ref dataAction);
				}
			}
		}

		private void CreateDataRegions(IReference<IScope> owner, ReportingServices.ReportIntermediateFormat.ReportItem reportItem, OnDemandProcessingContext odpContext, bool onePass, ref DataActions dataAction)
		{
			RuntimeDataTablixObj runtimeDataTablixObj = null;
			switch (reportItem.ObjectType)
			{
			case ReportingServices.ReportProcessing.ObjectType.Rectangle:
			{
				ReportingServices.ReportIntermediateFormat.ReportItemCollection reportItems = ((ReportingServices.ReportIntermediateFormat.Rectangle)reportItem).ReportItems;
				if (reportItems != null && reportItems.ComputedReportItems != null)
				{
					CreateDataRegions(owner, reportItems.ComputedReportItems, odpContext, onePass, ref dataAction);
				}
				break;
			}
			case ReportingServices.ReportProcessing.ObjectType.Tablix:
				runtimeDataTablixObj = new RuntimeTablixObj(owner, (ReportingServices.ReportIntermediateFormat.Tablix)reportItem, ref dataAction, odpContext, onePass);
				break;
			case ReportingServices.ReportProcessing.ObjectType.Chart:
				runtimeDataTablixObj = new RuntimeChartObj(owner, (ReportingServices.ReportIntermediateFormat.Chart)reportItem, ref dataAction, odpContext, onePass);
				break;
			case ReportingServices.ReportProcessing.ObjectType.GaugePanel:
				runtimeDataTablixObj = new RuntimeGaugePanelObj(owner, (GaugePanel)reportItem, ref dataAction, odpContext, onePass);
				break;
			case ReportingServices.ReportProcessing.ObjectType.MapDataRegion:
				runtimeDataTablixObj = new RuntimeMapDataRegionObj(owner, (MapDataRegion)reportItem, ref dataAction, odpContext, onePass);
				break;
			case ReportingServices.ReportProcessing.ObjectType.CustomReportItem:
				if (reportItem.IsDataRegion)
				{
					runtimeDataTablixObj = new RuntimeCriObj(owner, (ReportingServices.ReportIntermediateFormat.CustomReportItem)reportItem, ref dataAction, odpContext, onePass);
				}
				break;
			case ReportingServices.ReportProcessing.ObjectType.Map:
			{
				List<MapDataRegion> mapDataRegions = ((Map)reportItem).MapDataRegions;
				if (mapDataRegions != null)
				{
					CreateMapDataRegions(owner, mapDataRegions, odpContext, onePass, ref dataAction);
				}
				break;
			}
			}
			if (runtimeDataTablixObj != null)
			{
				AddDataRegion(runtimeDataTablixObj, (ReportingServices.ReportIntermediateFormat.DataRegion)reportItem);
			}
		}

		private void CreateMapDataRegions(IReference<IScope> owner, List<MapDataRegion> mapDataRegions, OnDemandProcessingContext odpContext, bool onePass, ref DataActions dataAction)
		{
			RuntimeDataTablixObj runtimeDataTablixObj = null;
			foreach (MapDataRegion mapDataRegion in mapDataRegions)
			{
				runtimeDataTablixObj = new RuntimeMapDataRegionObj(owner, mapDataRegion, ref dataAction, odpContext, onePass);
				AddDataRegion(runtimeDataTablixObj, mapDataRegion);
			}
		}

		private void AddDataRegion(RuntimeDataTablixObj dataRegion, ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef)
		{
			RuntimeDataTablixObjReference runtimeDataTablixObjReference = (RuntimeDataTablixObjReference)dataRegion.SelfReference;
			runtimeDataTablixObjReference.UnPinValue();
			int indexInCollection = dataRegionDef.IndexInCollection;
			ListUtils.AdjustLength(m_dataRegionObjs, indexInCollection);
			m_dataRegionObjs[indexInCollection] = runtimeDataTablixObjReference;
		}

		internal void FirstPassNextDataRow(OnDemandProcessingContext odpContext)
		{
			AggregateRowInfo aggregateRowInfo = AggregateRowInfo.CreateAndSaveAggregateInfo(odpContext);
			for (int i = 0; i < m_dataRegionObjs.Count; i++)
			{
				RuntimeDataRegionObjReference runtimeDataRegionObjReference = m_dataRegionObjs[i];
				if (runtimeDataRegionObjReference != null)
				{
					using (runtimeDataRegionObjReference.PinValue())
					{
						runtimeDataRegionObjReference.Value().NextRow();
					}
					aggregateRowInfo.RestoreAggregateInfo(odpContext);
				}
			}
		}

		internal void SortAndFilter(AggregateUpdateContext aggContext)
		{
			Traverse(ProcessingStages.SortAndFilter, aggContext);
		}

		private void Traverse(ProcessingStages operation, AggregateUpdateContext context)
		{
			for (int i = 0; i < m_dataRegionObjs.Count; i++)
			{
				RuntimeDataRegionObjReference runtimeDataRegionObjReference = m_dataRegionObjs[i];
				if (!(runtimeDataRegionObjReference != null))
				{
					continue;
				}
				using (runtimeDataRegionObjReference.PinValue())
				{
					switch (operation)
					{
					case ProcessingStages.SortAndFilter:
						runtimeDataRegionObjReference.Value().SortAndFilter(context);
						break;
					case ProcessingStages.UpdateAggregates:
						runtimeDataRegionObjReference.Value().UpdateAggregates(context);
						break;
					default:
						Global.Tracer.Assert(condition: false, "Unknown ProcessingStage in Traverse");
						break;
					}
				}
			}
		}

		internal void UpdateAggregates(AggregateUpdateContext aggContext)
		{
			Traverse(ProcessingStages.UpdateAggregates, aggContext);
		}

		internal void CalculateRunningValues(Dictionary<string, IReference<RuntimeGroupRootObj>> groupCollection, IReference<RuntimeGroupRootObj> lastGroup, AggregateUpdateContext aggContext)
		{
			for (int i = 0; i < m_dataRegionObjs.Count; i++)
			{
				RuntimeDataRegionObjReference runtimeDataRegionObjReference = m_dataRegionObjs[i];
				if (runtimeDataRegionObjReference != null)
				{
					using (runtimeDataRegionObjReference.PinValue())
					{
						runtimeDataRegionObjReference.Value().CalculateRunningValues(groupCollection, lastGroup, aggContext);
					}
				}
			}
		}

		internal static void StoreRunningValues(AggregatesImpl globalRVCol, List<ReportingServices.ReportIntermediateFormat.RunningValueInfo> runningValues, ref ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[] runningValueValues)
		{
			if (runningValues != null && 0 < runningValues.Count)
			{
				if (runningValueValues == null)
				{
					runningValueValues = new ReportingServices.ReportIntermediateFormat.DataAggregateObjResult[runningValues.Count];
				}
				for (int i = 0; i < runningValues.Count; i++)
				{
					ReportingServices.ReportIntermediateFormat.RunningValueInfo runningValueInfo = runningValues[i];
					ReportingServices.ReportIntermediateFormat.DataAggregateObj aggregateObj = globalRVCol.GetAggregateObj(runningValueInfo.Name);
					if (aggregateObj != null)
					{
						runningValueValues[i] = aggregateObj.AggregateResult();
					}
				}
			}
			else
			{
				runningValueValues = null;
			}
		}

		internal void CreateAllDataRegionInstances(ScopeInstance parentInstance, OnDemandProcessingContext odpContext, IReference<IScope> owner)
		{
			for (int i = 0; i < m_dataRegionObjs.Count; i++)
			{
				CreateDataRegionInstance(parentInstance, odpContext, m_dataRegionObjs[i]);
			}
			m_dataRegionObjs = null;
		}

		internal void CreateInstances(ScopeInstance parentInstance, OnDemandProcessingContext odpContext, IReference<IScope> owner, List<ReportingServices.ReportIntermediateFormat.ReportItem> reportItems)
		{
			if (reportItems != null)
			{
				for (int i = 0; i < reportItems.Count; i++)
				{
					CreateInstance(parentInstance, reportItems[i], odpContext, owner);
				}
			}
		}

		private static void CreateDataRegionInstance(ScopeInstance parentInstance, OnDemandProcessingContext odpContext, RuntimeDataRegionObjReference dataRegionObjRef)
		{
			if (!(dataRegionObjRef == null))
			{
				using (dataRegionObjRef.PinValue())
				{
					RuntimeDataTablixObj obj = (RuntimeDataTablixObj)dataRegionObjRef.Value();
					ReportingServices.ReportIntermediateFormat.DataRegion dataRegionDef = obj.DataRegionDef;
					obj.SetupEnvironment();
					DataRegionInstance dataRegionInstance = DataRegionInstance.CreateInstance(parentInstance, odpContext.OdpMetadata, dataRegionDef, odpContext.CurrentDataSetIndex).Value();
					obj.CreateInstances(dataRegionInstance);
					dataRegionInstance.InstanceComplete();
					dataRegionDef.RuntimeDataRegionObj = null;
				}
			}
		}

		public static void MergeDataProcessingItems(List<ReportingServices.ReportIntermediateFormat.ReportItem> candidateItems, ref List<ReportingServices.ReportIntermediateFormat.ReportItem> results)
		{
			if (candidateItems != null)
			{
				for (int i = 0; i < candidateItems.Count; i++)
				{
					MergeDataProcessingItem(candidateItems[i], ref results);
				}
			}
		}

		public static void MergeDataProcessingItem(ReportingServices.ReportIntermediateFormat.ReportItem item, ref List<ReportingServices.ReportIntermediateFormat.ReportItem> results)
		{
			if (item == null)
			{
				return;
			}
			if (item.IsDataRegion)
			{
				AddItem(item, ref results);
				return;
			}
			switch (item.ObjectType)
			{
			case ReportingServices.ReportProcessing.ObjectType.Rectangle:
				MergeDataProcessingItems(((ReportingServices.ReportIntermediateFormat.Rectangle)item).ReportItems.ComputedReportItems, ref results);
				break;
			case ReportingServices.ReportProcessing.ObjectType.Subreport:
			case ReportingServices.ReportProcessing.ObjectType.Map:
				AddItem(item, ref results);
				break;
			}
		}

		private static void AddItem(ReportingServices.ReportIntermediateFormat.ReportItem item, ref List<ReportingServices.ReportIntermediateFormat.ReportItem> results)
		{
			if (results == null)
			{
				results = new List<ReportingServices.ReportIntermediateFormat.ReportItem>();
			}
			results.Add(item);
		}

		private void CreateInstance(ScopeInstance parentInstance, ReportingServices.ReportIntermediateFormat.ReportItem reportItem, OnDemandProcessingContext odpContext, IReference<IScope> owner)
		{
			if (reportItem == null)
			{
				return;
			}
			if (reportItem.IsDataRegion)
			{
				ReportingServices.ReportIntermediateFormat.DataRegion dataRegion = (ReportingServices.ReportIntermediateFormat.DataRegion)reportItem;
				RuntimeDataRegionObjReference dataRegionObjRef = m_dataRegionObjs[dataRegion.IndexInCollection];
				CreateDataRegionInstance(parentInstance, odpContext, dataRegionObjRef);
				return;
			}
			switch (reportItem.ObjectType)
			{
			case ReportingServices.ReportProcessing.ObjectType.Subreport:
				CreateSubReportInstance((ReportingServices.ReportIntermediateFormat.SubReport)reportItem, parentInstance, odpContext, owner);
				break;
			case ReportingServices.ReportProcessing.ObjectType.Rectangle:
			{
				ReportingServices.ReportIntermediateFormat.Rectangle rectangle = (ReportingServices.ReportIntermediateFormat.Rectangle)reportItem;
				CreateInstances(parentInstance, odpContext, owner, rectangle.ReportItems.ComputedReportItems);
				break;
			}
			case ReportingServices.ReportProcessing.ObjectType.Map:
			{
				List<MapDataRegion> mapDataRegions = ((Map)reportItem).MapDataRegions;
				for (int i = 0; i < mapDataRegions.Count; i++)
				{
					CreateInstance(parentInstance, mapDataRegions[i], odpContext, owner);
				}
				break;
			}
			}
		}

		private void CreateSubReportInstance(ReportingServices.ReportIntermediateFormat.SubReport subReport, ScopeInstance parentInstance, OnDemandProcessingContext odpContext, IReference<IScope> owner)
		{
			if (subReport.ExceededMaxLevel)
			{
				return;
			}
			IReference<ReportingServices.ReportIntermediateFormat.SubReportInstance> reference2 = subReport.CurrentSubReportInstance = ReportingServices.ReportIntermediateFormat.SubReportInstance.CreateInstance(parentInstance, subReport, odpContext.OdpMetadata);
			subReport.OdpContext.UserSortFilterContext.CurrentContainingScope = owner;
			odpContext.LastTablixProcessingReportScope = parentInstance.RIFReportScope;
			if (SubReportInitializer.InitializeSubReport(subReport))
			{
				IReference<ReportingServices.ReportIntermediateFormat.ReportInstance> reportInstance = reference2.Value().ReportInstance;
				Merge.PreProcessTablixes(subReport.Report, subReport.OdpContext, !odpContext.ReprocessSnapshot);
				if (subReport.Report.HasSubReports)
				{
					SubReportInitializer.InitializeSubReports(subReport.Report, reportInstance.Value(), subReport.OdpContext, inDataRegion: false, fromCreateSubReportInstance: true);
				}
			}
			reference2?.Value().InstanceComplete();
			odpContext.EnsureCultureIsSetOnCurrentThread();
		}

		public RuntimeDataTablixObjReference GetDataRegionObj(ReportingServices.ReportIntermediateFormat.DataRegion rifDataRegion)
		{
			int indexInCollection = rifDataRegion.IndexInCollection;
			return m_dataRegionObjs[indexInCollection];
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			_ = writer.PersistenceHelper;
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.DataRegionObjs)
				{
					writer.Write(m_dataRegionObjs);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			_ = reader.PersistenceHelper;
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.DataRegionObjs)
				{
					m_dataRegionObjs = reader.ReadListOfRIFObjects<List<RuntimeDataTablixObjReference>>();
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
			}
		}

		public void ResolveReferences(Dictionary<ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRICollection;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.DataRegionObjs, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeDataTablixObjReference));
				return new Declaration(ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RuntimeRICollection, ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
